using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Complaints.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Application.Interfaces.Services;
using ComplaintMaintenanceService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.Exceptions;
using Shared.SharedLibrary.Services;

namespace ComplaintMaintenanceService.Application.Features.Complaints.Commands;

public class CancelComplaintCommand : IRequest<ComplaintDetailDto>
{
    public Guid ComplaintId { get; set; }
    public ComplaintCancelRequestDto Request { get; set; } = null!;
}

public class CancelComplaintCommandHandler
    : IRequestHandler<CancelComplaintCommand, ComplaintDetailDto>
{
    private readonly IComplaintRepository _complaintRepo;
    private readonly IRefTermRepository _refTermRepo;
    private readonly IStaffAvailabilityRepository _slotRepo;
    private readonly IComplaintProgressLogRepository _progressLogRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationGrpcClient _notificationClient;
    private readonly IIdentityGrpcClient _identityClient;
    private readonly ILogger<CancelComplaintCommandHandler> _logger;

    public CancelComplaintCommandHandler(
        IComplaintRepository complaintRepo,
        IRefTermRepository refTermRepo,
        IStaffAvailabilityRepository slotRepo,
        IComplaintProgressLogRepository progressLogRepo,
        ICurrentUserService currentUser,
        INotificationGrpcClient notificationClient,
        IIdentityGrpcClient identityClient,
        ILogger<CancelComplaintCommandHandler> logger
    )
    {
        _complaintRepo = complaintRepo;
        _refTermRepo = refTermRepo;
        _slotRepo = slotRepo;
        _progressLogRepo = progressLogRepo;
        _currentUser = currentUser;
        _notificationClient = notificationClient;
        _identityClient = identityClient;
        _logger = logger;
    }

    public async Task<ComplaintDetailDto> Handle(
        CancelComplaintCommand command,
        CancellationToken ct
    )
    {
        var complaint =
            await _complaintRepo.GetByIdAsync(command.ComplaintId, ct)
            ?? throw new NotFoundException(ComplaintConstants.Messages.ComplaintNotFound);

        if (complaint.ResidentId != _currentUser.UserId)
            throw new UnauthorizedAccessException();

        var cancelledStatus =
            await _refTermRepo.GetByCodeAsync(ComplaintConstants.StatusCodes.Cancelled)
            ?? throw new NotFoundException(ComplaintConstants.Messages.OpenStatusNotConfigured);

        if (complaint.StatusId == cancelledStatus.Id)
            throw new InvalidOperationException(ComplaintConstants.Messages.AlreadyCancelled);

        var now = DateTime.UtcNow;
        var residentId = _currentUser.UserId;

        if (complaint.ScheduledSlotId.HasValue)
        {
            var slot = await _slotRepo.GetByIdAsync(complaint.ScheduledSlotId.Value, ct);
            if (slot is not null)
            {
                slot.IsBooked = false;
                slot.IsCancelled = true;
                slot.ComplaintId = null;
                slot.UpdatedAt = now;
                slot.UpdatedBy = residentId;
                await _slotRepo.UpdateAsync(slot, ct);
            }
        }

        complaint.StatusId = cancelledStatus.Id;
        complaint.CancelledAt = now;
        complaint.CancellationReason = command.Request.CancellationReason;
        complaint.IsActive = false;
        complaint.UpdatedAt = now;
        complaint.UpdatedBy = residentId;

        await _complaintRepo.UpdateAsync(complaint, ct);

        var log = new ComplaintProgressLog
        {
            Id = Guid.NewGuid(),
            ComplaintId = complaint.Id,
            StatusId = cancelledStatus.Id,
            ChangedBy = residentId,
            ChangedDate = now,
            Remarks = command.Request.CancellationReason,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = residentId,
            UpdatedBy = residentId,
        };
        await _progressLogRepo.AddAsync(log, ct);

        _logger.LogInformation(
            "Complaint {ComplaintId} cancelled by resident {ResidentId}",
            complaint.Id,
            residentId
        );

        // Notify admins that the resident cancelled this complaint (email + in-app)
        var complaintId = complaint.Id;
        var cancellationReason = command.Request.CancellationReason;

        _ = Task.Run(async () =>
        {
            try
            {
                var admins = await _identityClient.GetUsersByRoleAsync(
                    ComplaintConstants.RoleCodes.Admin,
                    CancellationToken.None
                );

                foreach (var admin in admins)
                {
                    await _notificationClient.PushNotificationAsync(
                        admin.UserId,
                        ComplaintConstants.NotificationTypes.ComplaintCancelled, // FIXED: now "COMPLAINT_CANCELLED" to match DB template
                        ComplaintConstants.NotificationTitles.ComplaintCancelled,
                        string.Format(
                            ComplaintConstants.NotificationMessages.ComplaintCancelled,
                            complaintId,
                            cancellationReason
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
                    "Notification dispatch failed for cancellation on complaint {ComplaintId}",
                    complaintId
                );
            }
        });

        return new ComplaintDetailDto
        {
            ComplaintId = complaint.Id,
            ResidentId = complaint.ResidentId,
            Description = complaint.Description,
            Status = cancelledStatus.DisplayName,
            Priority = complaint.Priority?.DisplayName ?? string.Empty,
            Category = complaint.Category?.Name ?? string.Empty,
            ScheduledDate = complaint.ScheduledDate?.ToString(
                ComplaintConstants.DateFormats.OutputDate
            ),
            CreatedAt = complaint.CreatedAt,
            UpdatedAt = complaint.UpdatedAt,
            ScheduledSlotId = complaint.ScheduledSlotId,
            CancelledAt = complaint.CancelledAt,
            CancellationReason = complaint.CancellationReason,
        };
    }
}
