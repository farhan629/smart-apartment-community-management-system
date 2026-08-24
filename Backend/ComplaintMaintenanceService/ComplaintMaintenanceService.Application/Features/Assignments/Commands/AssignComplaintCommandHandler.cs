using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Assignments.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Application.Interfaces.Services;
using ComplaintMaintenanceService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.Exceptions;

namespace ComplaintMaintenanceService.Application.Features.Assignments.Commands;

public class AssignComplaintCommand : IRequest<AssignmentResponseDto>
{
    public Guid ComplaintId { get; set; }
    public Guid AssignedBy { get; set; }
    public AssignComplaintRequestDto Request { get; set; } = default!;
}

public class AssignComplaintCommandHandler
    : IRequestHandler<AssignComplaintCommand, AssignmentResponseDto>
{
    private readonly IComplaintRepository _complaintRepo;
    private readonly IComplaintAssignmentRepository _assignmentRepo;
    private readonly IComplaintProgressLogRepository _progressRepo;
    private readonly IStaffRepository _staffRepo;
    private readonly IRefTermRepository _refTermRepo;
    private readonly INotificationGrpcClient _notificationClient;
    private readonly IIdentityGrpcClient _identityClient;
    private readonly ILogger<AssignComplaintCommandHandler> _logger;

    public AssignComplaintCommandHandler(
        IComplaintRepository complaintRepo,
        IComplaintAssignmentRepository assignmentRepo,
        IComplaintProgressLogRepository progressRepo,
        IStaffRepository staffRepo,
        IRefTermRepository refTermRepo,
        INotificationGrpcClient notificationClient,
        IIdentityGrpcClient identityClient,
        ILogger<AssignComplaintCommandHandler> logger
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
        AssignComplaintCommand cmd,
        CancellationToken ct
    )
    {
        var complaint =
            await _complaintRepo.GetByIdAsync(cmd.ComplaintId, ct)
            ?? throw new NotFoundException(ComplaintConstants.Messages.ComplaintNotFound);

        var existing = await _assignmentRepo.GetActiveByComplaintIdAsync(cmd.ComplaintId, ct);
        if (existing is not null)
            throw new ConflictException(ComplaintConstants.AssignmentMessages.AlreadyAssigned);

        var staff =
            await _staffRepo.GetByIdAsync(cmd.Request.StaffId, ct)
            ?? throw new NotFoundException(ComplaintConstants.AssignmentMessages.StaffNotFound);

        var pendingStatus =
            await _refTermRepo.GetByCodeAndSetIdAsync(
                ComplaintConstants.AssignmentStatusCodes.PendingAcceptance,
                ComplaintConstants.RefSetIds.AssignmentStatus
            )
            ?? throw new BadRequestException(
                ComplaintConstants.AssignmentMessages.AssignmentStatusNotConfigured
            );

        var assignedComplaintStatus =
            await _refTermRepo.GetByCodeAndSetIdAsync(
                ComplaintConstants.StatusCodes.Assigned,
                ComplaintConstants.RefSetIds.ComplaintStatus
            ) ?? throw new BadRequestException(ComplaintConstants.Messages.OpenStatusNotConfigured);

        var now = DateTime.UtcNow;

        var assignment = new ComplaintAssignment
        {
            Id = Guid.NewGuid(),
            ComplaintId = cmd.ComplaintId,
            StaffId = cmd.Request.StaffId,
            AssignedBy = cmd.AssignedBy,
            StatusId = pendingStatus.Id,
            AssignedDate = now,
            DueDate = cmd.Request.DueDate,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = cmd.AssignedBy,
            UpdatedBy = cmd.AssignedBy,
        };

        await _assignmentRepo.AddAsync(assignment, ct);

        complaint.StatusId = assignedComplaintStatus.Id;
        complaint.UpdatedAt = now;
        complaint.UpdatedBy = cmd.AssignedBy;
        await _complaintRepo.UpdateAsync(complaint, ct);

        await _progressRepo.AddAsync(
            new ComplaintProgressLog
            {
                Id = Guid.NewGuid(),
                ComplaintId = cmd.ComplaintId,
                ChangedBy = cmd.AssignedBy,
                StatusId = assignedComplaintStatus.Id,
                Remarks = ComplaintConstants.AssignmentMessages.AssignmentCreated,
                ChangedDate = now,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = cmd.AssignedBy,
                UpdatedBy = cmd.AssignedBy,
            },
            ct
        );

        _logger.LogInformation(
            "Complaint {ComplaintId} assigned to staff {StaffId}",
            cmd.ComplaintId,
            cmd.Request.StaffId
        );

        var complaintId = cmd.ComplaintId;
        var staffUserId = staff.UserId;
        var residentId = complaint.ResidentId;

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

                if (staffUser is not null)
                {
                    await _notificationClient.PushNotificationAsync(
                        staffUser.UserId,
                        ComplaintConstants.NotificationTypes.ComplaintAssigned,
                        ComplaintConstants.NotificationTitles.ComplaintAssigned,
                        string.Format(
                            ComplaintConstants.NotificationMessages.ComplaintAssignedStaff,
                            complaintId
                        ),
                        complaintId,
                        staffUser.Email,
                        staffUser.Name,
                        CancellationToken.None
                    );
                }

                if (residentUser is not null)
                {
                    await _notificationClient.PushNotificationAsync(
                        residentUser.UserId,
                        ComplaintConstants.NotificationTypes.ComplaintAssigned,
                        ComplaintConstants.NotificationTitles.ComplaintAssigned,
                        string.Format(
                            ComplaintConstants.NotificationMessages.ComplaintAssignedUser,
                            complaintId,
                            staffName
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
                        ComplaintConstants.NotificationTypes.ComplaintAssigned,
                        ComplaintConstants.NotificationTitles.ComplaintAssigned,
                        string.Format(
                            ComplaintConstants.NotificationMessages.ComplaintAssignedUser,
                            complaintId,
                            staffName
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
                    "Notification dispatch failed for assignment on complaint {ComplaintId}",
                    complaintId
                );
            }
        });

        return new AssignmentResponseDto
        {
            AssignmentId = assignment.Id,
            ComplaintId = assignment.ComplaintId,
            StaffId = assignment.StaffId,
            StaffName = staff.Description,
            Status = pendingStatus.DisplayName,
            AssignedDate = assignment.AssignedDate,
            DueDate = assignment.DueDate,
            AssignedBy = assignment.AssignedBy,
        };
    }
}
