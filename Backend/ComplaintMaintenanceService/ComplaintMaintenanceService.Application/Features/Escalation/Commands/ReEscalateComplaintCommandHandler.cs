using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Escalation.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Application.Interfaces.Services;
using ComplaintMaintenanceService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.Exceptions;

namespace ComplaintMaintenanceService.Application.Features.Escalation.Commands;

public class ReEscalateComplaintCommand : IRequest<ReEscalateResponseDto>
{
    public Guid ComplaintId { get; set; }
    public Guid ResidentId { get; set; }
    public Guid AdminId { get; set; }
    public string EscalationReason { get; set; } = string.Empty;
}

public class ReEscalateComplaintCommandHandler
    : IRequestHandler<ReEscalateComplaintCommand, ReEscalateResponseDto>
{
    private readonly IComplaintEscalationRepository _escalationRepo;
    private readonly IComplaintRepository _complaintRepo;
    private readonly INotificationGrpcClient _notification;
    private readonly IIdentityGrpcClient _identityClient;
    private readonly ILogger<ReEscalateComplaintCommandHandler> _logger;

    public ReEscalateComplaintCommandHandler(
        IComplaintEscalationRepository escalationRepo,
        IComplaintRepository complaintRepo,
        INotificationGrpcClient notification,
        IIdentityGrpcClient identityClient,
        ILogger<ReEscalateComplaintCommandHandler> logger
    )
    {
        _escalationRepo = escalationRepo;
        _complaintRepo = complaintRepo;
        _notification = notification;
        _identityClient = identityClient;
        _logger = logger;
    }

    public async Task<ReEscalateResponseDto> Handle(
        ReEscalateComplaintCommand cmd,
        CancellationToken ct
    )
    {
        var complaint =
            await _complaintRepo.GetByIdAsync(cmd.ComplaintId, ct)
            ?? throw new NotFoundException(ComplaintConstants.Messages.ComplaintNotFound);

        if (complaint.ResidentId != cmd.ResidentId)
            throw new UnauthorizedAccessException(
                ComplaintConstants.EscalationMessages.UnauthorizedEscalation
            );

        var now = DateTime.UtcNow;

        var escalation = new ComplaintEscalation
        {
            Id = Guid.NewGuid(),
            ComplaintId = cmd.ComplaintId,
            EscalatedBy = cmd.ResidentId,
            EscalatedTo = cmd.ResidentId,
            EscalationReason = cmd.EscalationReason,
            EscalationDate = now,
            ResolvedAfterEscalation = false,
            ResolutionDate = null,
            CreatedAt = now,
            CreatedBy = cmd.ResidentId,
        };

        await _escalationRepo.AddAsync(escalation, ct);

        _logger.LogInformation(
            "Resident {ResidentId} escalated complaint {ComplaintId}",
            cmd.ResidentId,
            cmd.ComplaintId
        );

        var admins = await _identityClient.GetUsersByRoleAsync(
            ComplaintConstants.RoleCodes.Admin,
            ct
        );

        foreach (var admin in admins)
        {
            try
            {
                await _notification.PushNotificationAsync(
                    userId: admin.UserId,
                    notificationType: ComplaintConstants.NotificationTypes.ComplaintEscalated,
                    title: ComplaintConstants.NotificationTitles.ComplaintEscalated,
                    message: string.Format(
                        ComplaintConstants.NotificationMessages.ComplaintEscalated,
                        cmd.ComplaintId
                    ),
                    complaintId: cmd.ComplaintId,
                    recipientEmail: admin.Email,
                    recipientName: admin.Name,
                    ct: ct
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send escalation notification to admin {AdminId} for complaint {ComplaintId}",
                    admin.UserId,
                    cmd.ComplaintId
                );
            }
        }

        return new ReEscalateResponseDto
        {
            EscalationId = escalation.Id,
            ComplaintId = escalation.ComplaintId,
            EscalationReason = escalation.EscalationReason,
            EscalationDate = escalation.EscalationDate,
        };
    }
}
