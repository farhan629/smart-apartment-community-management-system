using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.BackgroundJobs.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Application.Interfaces.Services;
using ComplaintMaintenanceService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.Exceptions;

namespace ComplaintMaintenanceService.Application.Features.BackgroundJobs.Commands;

public class RunEscalationCheckCommand : IRequest<RunEscalationCheckResultDto>
{
    public Guid TriggeredBy { get; set; }
}

public class RunEscalationCheckCommandHandler
    : IRequestHandler<RunEscalationCheckCommand, RunEscalationCheckResultDto>
{
    private readonly IComplaintRepository _complaintRepo;
    private readonly IComplaintAssignmentRepository _assignmentRepo;
    private readonly IComplaintEscalationRepository _escalationRepo;
    private readonly IComplaintProgressLogRepository _progressRepo;
    private readonly IRefTermRepository _refTermRepo;
    private readonly INotificationGrpcClient _notificationClient;
    private readonly IIdentityGrpcClient _identityClient;
    private readonly ILogger<RunEscalationCheckCommandHandler> _logger;

    private static readonly TimeSpan EscalationThreshold = TimeSpan.FromHours(24);

    public RunEscalationCheckCommandHandler(
        IComplaintRepository complaintRepo,
        IComplaintAssignmentRepository assignmentRepo,
        IComplaintEscalationRepository escalationRepo,
        IComplaintProgressLogRepository progressRepo,
        IRefTermRepository refTermRepo,
        INotificationGrpcClient notificationClient,
        IIdentityGrpcClient identityClient,
        ILogger<RunEscalationCheckCommandHandler> logger
    )
    {
        _complaintRepo = complaintRepo;
        _assignmentRepo = assignmentRepo;
        _escalationRepo = escalationRepo;
        _progressRepo = progressRepo;
        _refTermRepo = refTermRepo;
        _notificationClient = notificationClient;
        _identityClient = identityClient;
        _logger = logger;
    }

    public async Task<RunEscalationCheckResultDto> Handle(
        RunEscalationCheckCommand cmd,
        CancellationToken ct
    )
    {
        var escalatedStatus =
            await _refTermRepo.GetByCodeAndSetIdAsync(
                ComplaintConstants.StatusCodes.Escalated,
                ComplaintConstants.RefSetIds.ComplaintStatus
            ) ?? throw new BadRequestException(ComplaintConstants.Messages.OpenStatusNotConfigured);

        var pendingAssignmentStatus =
            await _refTermRepo.GetByCodeAndSetIdAsync(
                ComplaintConstants.AssignmentStatusCodes.PendingAcceptance,
                ComplaintConstants.RefSetIds.AssignmentStatus
            )
            ?? throw new BadRequestException(
                ComplaintConstants.AssignmentMessages.AssignmentStatusNotConfigured
            );

        var now = DateTime.UtcNow;
        var cutoff = now - EscalationThreshold;
        var escalatedIds = new List<Guid>();

        var unresolved = await _escalationRepo.GetUnresolvedAsync(ct);
        var admins = await _identityClient.GetUsersByRoleAsync(
            ComplaintConstants.RoleCodes.Admin,
            ct
        );

        foreach (var existing in unresolved)
        {
            if (existing.Complaint is null)
                continue;

            var complaint = existing.Complaint;
            complaint.StatusId = escalatedStatus.Id;
            complaint.UpdatedAt = now;
            complaint.UpdatedBy = cmd.TriggeredBy;
            await _complaintRepo.UpdateAsync(complaint, ct);

            await _progressRepo.AddAsync(
                new ComplaintProgressLog
                {
                    Id = Guid.NewGuid(),
                    ComplaintId = complaint.Id,
                    ChangedBy = cmd.TriggeredBy,
                    StatusId = escalatedStatus.Id,
                    Remarks = ComplaintConstants.EscalationMessages.EscalationUpdated,
                    ChangedDate = now,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = cmd.TriggeredBy,
                    UpdatedBy = cmd.TriggeredBy,
                },
                ct
            );

            escalatedIds.Add(complaint.Id);

            foreach (var admin in admins)
            {
                try
                {
                    await _notificationClient.PushNotificationAsync(
                        admin.UserId,
                        ComplaintConstants.NotificationTypes.ComplaintEscalated,
                        ComplaintConstants.NotificationTitles.ComplaintEscalated,
                        string.Format(
                            ComplaintConstants.NotificationMessages.ComplaintEscalated,
                            complaint.Id
                        ),
                        complaint.Id,
                        admin.Email,
                        admin.Name,
                        ct
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Notification failed for escalation on complaint {ComplaintId}",
                        complaint.Id
                    );
                }
            }

            _logger.LogInformation("Complaint {ComplaintId} escalated", complaint.Id);
        }

        return new RunEscalationCheckResultDto
        {
            EscalatedCount = escalatedIds.Count,
            EscalatedComplaintIds = escalatedIds,
        };
    }
}
