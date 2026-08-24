using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Assignments.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Application.Interfaces.Services;
using ComplaintMaintenanceService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.Exceptions;

namespace ComplaintMaintenanceService.Application.Features.Assignments.Commands;

public class ReassignComplaintCommand : IRequest<AssignmentResponseDto>
{
    public Guid ComplaintId { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid AssignedBy { get; set; }
    public AssignComplaintRequestDto Request { get; set; } = default!;
}

public class ReassignComplaintCommandHandler
    : IRequestHandler<ReassignComplaintCommand, AssignmentResponseDto>
{
    private readonly IComplaintRepository _complaintRepo;
    private readonly IComplaintAssignmentRepository _assignmentRepo;
    private readonly IComplaintProgressLogRepository _progressRepo;
    private readonly IStaffRepository _staffRepo;
    private readonly IRefTermRepository _refTermRepo;
    private readonly INotificationGrpcClient _notificationClient;
    private readonly IIdentityGrpcClient _identityClient;
    private readonly ILogger<ReassignComplaintCommandHandler> _logger;

    public ReassignComplaintCommandHandler(
        IComplaintRepository complaintRepo,
        IComplaintAssignmentRepository assignmentRepo,
        IComplaintProgressLogRepository progressRepo,
        IStaffRepository staffRepo,
        IRefTermRepository refTermRepo,
        INotificationGrpcClient notificationClient,
        IIdentityGrpcClient identityClient,
        ILogger<ReassignComplaintCommandHandler> logger
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
        ReassignComplaintCommand cmd,
        CancellationToken ct
    )
    {
        var oldAssignment =
            await _assignmentRepo.GetByIdAsync(cmd.AssignmentId, ct)
            ?? throw new NotFoundException(
                ComplaintConstants.AssignmentMessages.AssignmentNotFound
            );

        if (oldAssignment.ComplaintId != cmd.ComplaintId)
            throw new BadRequestException(ComplaintConstants.AssignmentMessages.InvalidAssignment);

        var complaint =
            await _complaintRepo.GetByIdAsync(cmd.ComplaintId, ct)
            ?? throw new NotFoundException(ComplaintConstants.Messages.ComplaintNotFound);

        var newStaff =
            await _staffRepo.GetByIdAsync(cmd.Request.StaffId, ct)
            ?? throw new NotFoundException(ComplaintConstants.AssignmentMessages.StaffNotFound);

        var reassignedStatus =
            await _refTermRepo.GetByCodeAndSetIdAsync(
                ComplaintConstants.AssignmentStatusCodes.Reassigned,
                ComplaintConstants.RefSetIds.AssignmentStatus
            )
            ?? throw new BadRequestException(
                ComplaintConstants.AssignmentMessages.AssignmentStatusNotConfigured
            );

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

        oldAssignment.StatusId = reassignedStatus.Id;
        oldAssignment.IsActive = false;
        oldAssignment.UpdatedAt = now;
        oldAssignment.UpdatedBy = cmd.AssignedBy;
        await _assignmentRepo.UpdateAsync(oldAssignment, ct);

        var newAssignment = new ComplaintAssignment
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

        await _assignmentRepo.AddAsync(newAssignment, ct);

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
                Remarks = ComplaintConstants.AssignmentMessages.AssignmentReassigned,
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
            "Complaint {ComplaintId} reassigned to staff {StaffId}",
            cmd.ComplaintId,
            cmd.Request.StaffId
        );

        _ = Task.Run(
            async () =>
            {
                try
                {
                    var newStaffUser = await _identityClient.GetUserByIdAsync(newStaff.UserId, ct);
                    var residentUser = await _identityClient.GetUserByIdAsync(
                        complaint.ResidentId,
                        ct
                    );
                    var admins = await _identityClient.GetUsersByRoleAsync(
                        ComplaintConstants.RoleCodes.Admin,
                        ct
                    );

                    if (newStaffUser is not null)
                    {
                        await _notificationClient.PushNotificationAsync(
                            newStaffUser.UserId,
                            ComplaintConstants.NotificationTypes.ComplaintAssigned,
                            ComplaintConstants.NotificationTitles.ComplaintAssigned,
                            string.Format(
                                ComplaintConstants.NotificationMessages.ComplaintAssignedStaff,
                                cmd.ComplaintId
                            ),
                            cmd.ComplaintId,
                            newStaffUser.Email,
                            newStaffUser.Name,
                            ct
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
                                cmd.ComplaintId,
                                newStaffUser?.Name ?? string.Empty
                            ),
                            cmd.ComplaintId,
                            residentUser.Email,
                            residentUser.Name,
                            ct
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
                                cmd.ComplaintId,
                                newStaffUser?.Name ?? string.Empty
                            ),
                            cmd.ComplaintId,
                            admin.Email,
                            admin.Name,
                            ct
                        );
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Notification dispatch failed for reassign on complaint {ComplaintId}",
                        cmd.ComplaintId
                    );
                }
            },
            ct
        );

        return new AssignmentResponseDto
        {
            AssignmentId = newAssignment.Id,
            ComplaintId = newAssignment.ComplaintId,
            StaffId = newAssignment.StaffId,
            StaffName = newStaff.Description,
            Status = pendingStatus.DisplayName,
            AssignedDate = newAssignment.AssignedDate,
            DueDate = newAssignment.DueDate,
            AssignedBy = newAssignment.AssignedBy,
        };
    }
}
