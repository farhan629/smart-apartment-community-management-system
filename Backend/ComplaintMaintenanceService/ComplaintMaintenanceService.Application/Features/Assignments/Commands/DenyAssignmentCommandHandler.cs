using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Assignments.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Application.Interfaces.Services;
using ComplaintMaintenanceService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.Exceptions;

namespace ComplaintMaintenanceService.Application.Features.Assignments.Commands;

public class DenyAssignmentCommand : IRequest<AssignmentResponseDto>
{
    public Guid ComplaintId { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid StaffUserId { get; set; }
    public DenyAssignmentRequestDto Request { get; set; } = default!;
}

public class DenyAssignmentCommandHandler
    : IRequestHandler<DenyAssignmentCommand, AssignmentResponseDto>
{
    private readonly IComplaintRepository _complaintRepo;
    private readonly IComplaintAssignmentRepository _assignmentRepo;
    private readonly IComplaintProgressLogRepository _progressRepo;
    private readonly IStaffRepository _staffRepo;
    private readonly IRefTermRepository _refTermRepo;
    private readonly INotificationGrpcClient _notificationClient;
    private readonly IIdentityGrpcClient _identityClient;
    private readonly ILogger<DenyAssignmentCommandHandler> _logger;

    public DenyAssignmentCommandHandler(
        IComplaintRepository complaintRepo,
        IComplaintAssignmentRepository assignmentRepo,
        IComplaintProgressLogRepository progressRepo,
        IStaffRepository staffRepo,
        IRefTermRepository refTermRepo,
        INotificationGrpcClient notificationClient,
        IIdentityGrpcClient identityClient,
        ILogger<DenyAssignmentCommandHandler> logger
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

    public async Task<AssignmentResponseDto> Handle(DenyAssignmentCommand cmd, CancellationToken ct)
    {
        var assignment =
            await _assignmentRepo.GetByIdAsync(cmd.AssignmentId, ct)
            ?? throw new NotFoundException(
                ComplaintConstants.AssignmentMessages.AssignmentNotFound
            );

        if (assignment.ComplaintId != cmd.ComplaintId)
            throw new BadRequestException(ComplaintConstants.AssignmentMessages.InvalidAssignment);

        var deniedStatus =
            await _refTermRepo.GetByCodeAndSetIdAsync(
                ComplaintConstants.AssignmentStatusCodes.Denied,
                ComplaintConstants.RefSetIds.AssignmentStatus
            )
            ?? throw new BadRequestException(
                ComplaintConstants.AssignmentMessages.AssignmentStatusNotConfigured
            );

        var openStatus =
            await _refTermRepo.GetByCodeAndSetIdAsync(
                ComplaintConstants.StatusCodes.Open,
                ComplaintConstants.RefSetIds.ComplaintStatus
            ) ?? throw new BadRequestException(ComplaintConstants.Messages.OpenStatusNotConfigured);

        var now = DateTime.UtcNow;
        assignment.StatusId = deniedStatus.Id;
        assignment.DeniedDate = now;
        assignment.DenialReason = cmd.Request.DenialReason;
        assignment.IsActive = false;
        assignment.UpdatedAt = now;
        assignment.UpdatedBy = cmd.StaffUserId;
        await _assignmentRepo.UpdateAsync(assignment, ct);

        var complaint = await _complaintRepo.GetByIdAsync(cmd.ComplaintId, ct);
        if (complaint is not null)
        {
            complaint.StatusId = openStatus.Id;
            complaint.UpdatedAt = now;
            complaint.UpdatedBy = cmd.StaffUserId;
            await _complaintRepo.UpdateAsync(complaint, ct);
        }

        var staff = await _staffRepo.GetByIdAsync(assignment.StaffId, ct);

        await _progressRepo.AddAsync(
            new ComplaintProgressLog
            {
                Id = Guid.NewGuid(),
                ComplaintId = cmd.ComplaintId,
                ChangedBy = cmd.StaffUserId,
                StatusId = openStatus.Id,
                Remarks = ComplaintConstants.AssignmentMessages.AssignmentDenied,
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
            "Assignment {AssignmentId} denied for complaint {ComplaintId}",
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
                        ComplaintConstants.NotificationTypes.ComplaintDenied,
                        ComplaintConstants.NotificationTitles.ComplaintDenied,
                        string.Format(
                            ComplaintConstants.NotificationMessages.ComplaintDenied,
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
                        ComplaintConstants.NotificationTypes.ComplaintDenied,
                        ComplaintConstants.NotificationTitles.ComplaintDenied,
                        string.Format(
                            ComplaintConstants.NotificationMessages.ComplaintDenied,
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
                    "Notification dispatch failed for deny on assignment {AssignmentId}",
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
            Status = deniedStatus.DisplayName,
            AssignedDate = assignment.AssignedDate,
            DueDate = assignment.DueDate,
            DeniedDate = assignment.DeniedDate,
            DenialReason = assignment.DenialReason,
            AssignedBy = assignment.AssignedBy,
        };
    }
}
