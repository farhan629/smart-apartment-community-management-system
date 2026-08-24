using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Assignments.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Application.Interfaces.Services;
using ComplaintMaintenanceService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.Exceptions;

namespace ComplaintMaintenanceService.Application.Features.Assignments.Commands;

public class AcceptAssignmentCommand : IRequest<AssignmentResponseDto>
{
    public Guid ComplaintId { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid StaffUserId { get; set; }
}

public class AcceptAssignmentCommandHandler
    : IRequestHandler<AcceptAssignmentCommand, AssignmentResponseDto>
{
    private readonly IComplaintRepository _complaintRepo;
    private readonly IComplaintAssignmentRepository _assignmentRepo;
    private readonly IComplaintProgressLogRepository _progressRepo;
    private readonly IStaffRepository _staffRepo;
    private readonly IRefTermRepository _refTermRepo;
    private readonly INotificationGrpcClient _notificationClient;
    private readonly IIdentityGrpcClient _identityClient;
    private readonly ILogger<AcceptAssignmentCommandHandler> _logger;

    public AcceptAssignmentCommandHandler(
        IComplaintRepository complaintRepo,
        IComplaintAssignmentRepository assignmentRepo,
        IComplaintProgressLogRepository progressRepo,
        IStaffRepository staffRepo,
        IRefTermRepository refTermRepo,
        INotificationGrpcClient notificationClient,
        IIdentityGrpcClient identityClient,
        ILogger<AcceptAssignmentCommandHandler> logger
    )
    {
        _complaintRepo = complaintRepo;
        _assignmentRepo = assignmentRepo;
        _progressRepo = progressRepo;
        _staffRepo = staffRepo;
        _refTermRepo = refTermRepo;
        _notificationClient = notificationClient;
        _identityClient = identityClient;
        _logger = logger;
    }

    public async Task<AssignmentResponseDto> Handle(
        AcceptAssignmentCommand cmd,
        CancellationToken ct
    )
    {
        var assignment =
            await _assignmentRepo.GetByIdAsync(cmd.AssignmentId, ct)
            ?? throw new NotFoundException(
                ComplaintConstants.AssignmentMessages.AssignmentNotFound
            );

        if (assignment.ComplaintId != cmd.ComplaintId)
            throw new BadRequestException(ComplaintConstants.AssignmentMessages.InvalidAssignment);

        var activeStatus =
            await _refTermRepo.GetByCodeAndSetIdAsync(
                ComplaintConstants.AssignmentStatusCodes.Active,
                ComplaintConstants.RefSetIds.AssignmentStatus
            )
            ?? throw new BadRequestException(
                ComplaintConstants.AssignmentMessages.AssignmentStatusNotConfigured
            );

        if (assignment.StatusId == activeStatus.Id)
            throw new ConflictException(ComplaintConstants.AssignmentMessages.AlreadyActioned);

        var now = DateTime.UtcNow;
        assignment.StatusId = activeStatus.Id;
        assignment.AcceptedDate = now;
        assignment.UpdatedAt = now;
        assignment.UpdatedBy = cmd.StaffUserId;
        await _assignmentRepo.UpdateAsync(assignment, ct);

        var complaint = await _complaintRepo.GetByIdAsync(cmd.ComplaintId, ct);
        var staff = await _staffRepo.GetByIdAsync(assignment.StaffId, ct);

        await _progressRepo.AddAsync(
            new ComplaintProgressLog
            {
                Id = Guid.NewGuid(),
                ComplaintId = cmd.ComplaintId,
                ChangedBy = cmd.StaffUserId,
                StatusId = complaint!.StatusId,
                Remarks = ComplaintConstants.AssignmentMessages.AssignmentAccepted,
                ChangedDate = now,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = cmd.StaffUserId,
                UpdatedBy = cmd.StaffUserId,
            },
            ct
        );

        _logger.LogInformation(
            "Assignment {AssignmentId} accepted for complaint {ComplaintId}",
            cmd.AssignmentId,
            cmd.ComplaintId
        );

        var complaintId = cmd.ComplaintId;
        var staffUserId = staff?.UserId ?? Guid.Empty;
        var residentId = complaint?.ResidentId ?? Guid.Empty;

        _ = Task.Run(async () =>
        {
            try
            {
                var staffUser = await _identityClient.GetUserByIdAsync(
                    staffUserId,
                    CancellationToken.None
                );
                var residentUser = await _identityClient.GetUserByIdAsync(
                    residentId,
                    CancellationToken.None
                );
                var admins = await _identityClient.GetUsersByRoleAsync(
                    ComplaintConstants.RoleCodes.Admin,
                    CancellationToken.None
                );
                var staffName = staffUser?.Name ?? string.Empty;

                if (residentUser is not null)
                {
                    await _notificationClient.PushNotificationAsync(
                        residentUser.UserId,
                        ComplaintConstants.NotificationTypes.ComplaintAccepted,
                        ComplaintConstants.NotificationTitles.ComplaintAccepted,
                        string.Format(
                            ComplaintConstants.NotificationMessages.ComplaintAccepted,
                            staffName,
                            complaintId
                        ),
                        complaintId,
                        residentUser.Email,
                        residentUser.Name,
                        CancellationToken.None
                    );
                }

                foreach (var admin in admins)
                {
                    await _notificationClient.PushNotificationAsync(
                        admin.UserId,
                        ComplaintConstants.NotificationTypes.ComplaintAccepted,
                        ComplaintConstants.NotificationTitles.ComplaintAccepted,
                        string.Format(
                            ComplaintConstants.NotificationMessages.ComplaintAccepted,
                            staffName,
                            complaintId
                        ),
                        complaintId,
                        admin.Email,
                        admin.Name,
                        CancellationToken.None
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Notification dispatch failed for accept on assignment {AssignmentId}",
                    cmd.AssignmentId
                );
            }
        });

        return new AssignmentResponseDto
        {
            AssignmentId = assignment.Id,
            ComplaintId = assignment.ComplaintId,
            StaffId = assignment.StaffId,
            StaffName = staff?.Description ?? string.Empty,
            Status = activeStatus.DisplayName,
            AssignedDate = assignment.AssignedDate,
            DueDate = assignment.DueDate,
            AcceptedDate = assignment.AcceptedDate,
            AssignedBy = assignment.AssignedBy,
        };
    }
}
