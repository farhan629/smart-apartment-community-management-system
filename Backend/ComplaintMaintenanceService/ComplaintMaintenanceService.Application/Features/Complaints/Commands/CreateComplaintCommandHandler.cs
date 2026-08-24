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

public class CreateComplaintCommand : IRequest<ComplaintResponseDto>
{
    public CreateComplaintRequestDto Request { get; set; } = null!;
}

public class CreateComplaintCommandHandler
    : IRequestHandler<CreateComplaintCommand, ComplaintResponseDto>
{
    private readonly IComplaintRepository _complaintRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IRefTermRepository _refTermRepo;
    private readonly ICurrentUserService _currentUser;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CreateComplaintCommandHandler> _logger;

    public CreateComplaintCommandHandler(
        IComplaintRepository complaintRepo,
        ICategoryRepository categoryRepo,
        IRefTermRepository refTermRepo,
        ICurrentUserService currentUser,
        IServiceScopeFactory scopeFactory,
        ILogger<CreateComplaintCommandHandler> logger
    )
    {
        _complaintRepo = complaintRepo;
        _categoryRepo = categoryRepo;
        _refTermRepo = refTermRepo;
        _currentUser = currentUser;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<ComplaintResponseDto> Handle(
        CreateComplaintCommand command,
        CancellationToken ct
    )
    {
        var dto = command.Request;

        var category =
            await _categoryRepo.GetByIdAsync(dto.CategoryId)
            ?? throw new NotFoundException(ComplaintConstants.Messages.CategoryNotFound);

        var complaintType =
            await _refTermRepo.GetByIdAsync(dto.ComplaintTypeRefId)
            ?? throw new NotFoundException(ComplaintConstants.Messages.InvalidRefTerm);

        var priority =
            await _refTermRepo.GetByIdAsync(dto.PriorityRefId)
            ?? throw new NotFoundException(ComplaintConstants.Messages.InvalidRefTerm);

        var openStatus =
            await _refTermRepo.GetByCodeAndSetIdAsync(
                ComplaintConstants.StatusCodes.Open,
                ComplaintConstants.RefSetIds.ComplaintStatus
            ) ?? throw new NotFoundException(ComplaintConstants.Messages.OpenStatusNotConfigured);

        DateTime? scheduledDate = DateOnly.TryParse(dto.PreferredDate, out var d)
            ? d.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)
            : null;

        TimeSpan? scheduledTime = TimeOnly.TryParse(dto.PreferredTime, out var t)
            ? t.ToTimeSpan()
            : null;

        var now = DateTime.UtcNow;
        var residentId = _currentUser.UserId;

        var complaint = new Complaint
        {
            Id = Guid.NewGuid(),
            ResidentId = residentId,
            ComplaintTypeId = dto.ComplaintTypeRefId,
            CategoryId = dto.CategoryId,
            PriorityId = dto.PriorityRefId,
            StatusId = openStatus.Id,
            Description = dto.Description.Trim(),
            ScheduledDate = scheduledDate,
            ScheduledTime = scheduledTime,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = residentId,
            UpdatedBy = residentId,
        };

        await _complaintRepo.AddAsync(complaint, ct);

        _logger.LogInformation(
            "Complaint {ComplaintId} submitted by resident {ResidentId}",
            complaint.Id,
            residentId
        );

        DispatchCreateComplaintNotifications(complaint.Id, dto.CategoryId, category.Name);

        return new ComplaintResponseDto
        {
            ComplaintId = complaint.Id,
            ResidentId = residentId,
            Description = complaint.Description,
            ComplaintType = complaintType.DisplayName,
            Category = category.Name,
            CategoryId = category.Id,
            CategoryImg = category.Img,
            Priority = priority.DisplayName,
            Status = openStatus.DisplayName,
            ScheduledDate = dto.PreferredDate,
            ScheduledTime = dto.PreferredTime,
            CreatedAt = now,
        };
    }

    /// <summary>
    /// Dispatches notifications on an independent DI scope and CancellationToken.None,
    /// so the work survives even after the HTTP response is sent and the request's
    /// own scope/token are torn down. Failures are logged only � never surfaced to the caller.
    /// </summary>
    private void DispatchCreateComplaintNotifications(
        Guid complaintId,
        Guid categoryId,
        string categoryName
    )
    {
        _ = Task.Run(async () =>
        {
            using var scope = _scopeFactory.CreateScope();
            var identityClient = scope.ServiceProvider.GetRequiredService<IIdentityGrpcClient>();
            var notificationClient =
                scope.ServiceProvider.GetRequiredService<INotificationGrpcClient>();
            var staffRepo = scope.ServiceProvider.GetRequiredService<IStaffRepository>();
            var logger = scope.ServiceProvider.GetRequiredService<
                ILogger<CreateComplaintCommandHandler>
            >();

            try
            {
                var admins = await identityClient.GetUsersByRoleAsync(
                    ComplaintConstants.RoleCodes.Admin,
                    CancellationToken.None
                );
                logger.LogInformation(
                    "Notification dispatch: found {Count} admin(s) for complaint {ComplaintId}",
                    admins.Count,
                    complaintId
                );

                foreach (var admin in admins)
                {
                    await notificationClient.PushNotificationAsync(
                        userId: admin.UserId,
                        notificationType: ComplaintConstants.NotificationTypes.ComplaintRaised,
                        title: ComplaintConstants.NotificationTitles.ComplaintRaised,
                        message: string.Format(
                            ComplaintConstants.NotificationMessages.ComplaintRaisedAdmin,
                            complaintId,
                            categoryName
                        ),
                        complaintId: complaintId,
                        recipientEmail: admin.Email,
                        recipientName: admin.Name,
                        ct: CancellationToken.None
                    );
                }

                var staffList = await staffRepo.GetByCategoryIdAsync(
                    categoryId,
                    CancellationToken.None
                );
                logger.LogInformation(
                    "Notification dispatch: found {Count} staff in category {CategoryId} for complaint {ComplaintId}",
                    staffList.Count,
                    categoryId,
                    complaintId
                );

                foreach (var staff in staffList)
                {
                    var staffUser = await identityClient.GetUserByIdAsync(
                        staff.UserId,
                        CancellationToken.None
                    );
                    if (staffUser is null)
                    {
                        logger.LogWarning(
                            "Notification dispatch: IdentityService returned no user for staff {StaffUserId}",
                            staff.UserId
                        );
                        continue;
                    }

                    await notificationClient.PushNotificationAsync(
                        userId: staff.UserId,
                        notificationType: ComplaintConstants.NotificationTypes.ComplaintRaised,
                        title: ComplaintConstants.NotificationTitles.ComplaintRaised,
                        message: string.Format(
                            ComplaintConstants.NotificationMessages.ComplaintRaisedStaff,
                            complaintId,
                            categoryName
                        ),
                        complaintId: complaintId,
                        recipientEmail: staffUser.Email,
                        recipientName: staffUser.Name,
                        ct: CancellationToken.None
                    );
                }

                logger.LogInformation(
                    "Notification dispatch completed for complaint {ComplaintId}",
                    complaintId
                );
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Notification dispatch failed for complaint {ComplaintId}",
                    complaintId
                );
            }
        });
    }
}
