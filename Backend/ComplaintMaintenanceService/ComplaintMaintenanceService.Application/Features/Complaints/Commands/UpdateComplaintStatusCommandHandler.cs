using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Complaints.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Application.Interfaces.Services;
using ComplaintMaintenanceService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.Exceptions;
using Shared.SharedLibrary.Services;

namespace ComplaintMaintenanceService.Application.Features.Complaints.Commands;

public class UpdateComplaintStatusCommand : IRequest<ComplaintDetailDto>
{
    public Guid ComplaintId { get; set; }
    public ComplaintStatusUpdateRequestDto Request { get; set; } = null!;
}

public class UpdateComplaintStatusCommandHandler
    : IRequestHandler<UpdateComplaintStatusCommand, ComplaintDetailDto>
{
    private readonly IComplaintRepository _complaintRepo;
    private readonly IRefTermRepository _refTermRepo;
    private readonly IComplaintProgressLogRepository _progressLogRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UpdateComplaintStatusCommandHandler> _logger;

    public UpdateComplaintStatusCommandHandler(
        IComplaintRepository complaintRepo,
        IRefTermRepository refTermRepo,
        IComplaintProgressLogRepository progressLogRepo,
        ICurrentUserService currentUser,
        IServiceScopeFactory scopeFactory,
        ILogger<UpdateComplaintStatusCommandHandler> logger
    )
    {
        _complaintRepo = complaintRepo;
        _refTermRepo = refTermRepo;
        _progressLogRepo = progressLogRepo;
        _currentUser = currentUser;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<ComplaintDetailDto> Handle(
        UpdateComplaintStatusCommand command,
        CancellationToken ct
    )
    {
        var complaint =
            await _complaintRepo.GetByIdAsync(command.ComplaintId, ct)
            ?? throw new NotFoundException(ComplaintConstants.Messages.ComplaintNotFound);

        var newStatus =
            await _refTermRepo.GetByCodeAndSetIdAsync(
                command.Request.Status,
                ComplaintConstants.RefSetIds.ComplaintStatus
            ) ?? throw new NotFoundException(ComplaintConstants.Messages.InvalidStatusValue);

        var allowedCodes = new[]
        {
            ComplaintConstants.StatusCodes.InProgress,
            ComplaintConstants.StatusCodes.Resolved,
        };
        if (!allowedCodes.Contains(command.Request.Status))
            throw new BadRequestException(ComplaintConstants.Messages.CannotUpdateStatus);

        var now = DateTime.UtcNow;
        var staffId = _currentUser.UserId;

        complaint.StatusId = newStatus.Id;
        complaint.UpdatedAt = now;
        complaint.UpdatedBy = staffId;

        await _complaintRepo.UpdateAsync(complaint, ct);

        await _progressLogRepo.AddAsync(
            new ComplaintProgressLog
            {
                Id = Guid.NewGuid(),
                ComplaintId = complaint.Id,
                StatusId = newStatus.Id,
                ChangedBy = staffId,
                ChangedDate = now,
                Remarks = $"Status updated to {newStatus.DisplayName}",
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = staffId,
                UpdatedBy = staffId,
            },
            ct
        );

        _logger.LogInformation(
            "Complaint {ComplaintId} status updated to {Status} by {StaffId}",
            complaint.Id,
            newStatus.DisplayName,
            staffId
        );

        var residentId = complaint.ResidentId;
        var complaintId = complaint.Id;
        var statusCode = command.Request.Status;

        _ = Task.Run(async () =>
        {
            using var scope = _scopeFactory.CreateScope();
            var identityClient = scope.ServiceProvider.GetRequiredService<IIdentityGrpcClient>();
            var notificationClient =
                scope.ServiceProvider.GetRequiredService<INotificationGrpcClient>();

            try
            {
                var residentUser = await identityClient.GetUserByIdAsync(
                    residentId,
                    CancellationToken.None
                );
                var admins = await identityClient.GetUsersByRoleAsync(
                    ComplaintConstants.RoleCodes.Admin,
                    CancellationToken.None
                );

                if (statusCode == ComplaintConstants.StatusCodes.InProgress)
                {
                    if (residentUser is not null)
                    {
                        await notificationClient.PushNotificationAsync(
                            residentUser.UserId,
                            ComplaintConstants.NotificationTypes.ComplaintInProgress,
                            ComplaintConstants.NotificationTitles.ComplaintInProgress,
                            string.Format(
                                ComplaintConstants.NotificationMessages.ComplaintInProgress,
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
                        await notificationClient.PushNotificationAsync(
                            admin.UserId,
                            ComplaintConstants.NotificationTypes.ComplaintInProgress,
                            ComplaintConstants.NotificationTitles.ComplaintInProgress,
                            string.Format(
                                ComplaintConstants.NotificationMessages.ComplaintInProgress,
                                complaintId
                            ),
                            complaintId,
                            admin.Email,
                            admin.Name,
                            CancellationToken.None
                        );
                    }
                }
                else if (statusCode == ComplaintConstants.StatusCodes.Resolved)
                {
                    if (residentUser is not null)
                    {
                        await notificationClient.PushNotificationAsync(
                            residentUser.UserId,
                            ComplaintConstants.NotificationTypes.ComplaintResolved,
                            ComplaintConstants.NotificationTitles.ComplaintResolved,
                            string.Format(
                                ComplaintConstants.NotificationMessages.ComplaintResolved,
                                complaintId
                            ),
                            complaintId,
                            residentUser.Email,
                            residentUser.Name,
                            CancellationToken.None
                        );

                        await notificationClient.PushNotificationAsync(
                            residentUser.UserId,
                            ComplaintConstants.NotificationTypes.ComplaintRatingRequest,
                            ComplaintConstants.NotificationTitles.ComplaintRatingRequest,
                            string.Format(
                                ComplaintConstants.NotificationMessages.ComplaintRatingRequest,
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
                        await notificationClient.PushNotificationAsync(
                            admin.UserId,
                            ComplaintConstants.NotificationTypes.ComplaintResolved,
                            ComplaintConstants.NotificationTitles.ComplaintResolved,
                            string.Format(
                                ComplaintConstants.NotificationMessages.ComplaintResolved,
                                complaintId
                            ),
                            complaintId,
                            admin.Email,
                            admin.Name,
                            CancellationToken.None
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Notification dispatch failed for status update on complaint {ComplaintId}",
                    complaintId
                );
            }
        });

        return new ComplaintDetailDto
        {
            ComplaintId = complaint.Id,
            ResidentId = complaint.ResidentId,
            Description = complaint.Description,
            Status = newStatus.DisplayName,
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
