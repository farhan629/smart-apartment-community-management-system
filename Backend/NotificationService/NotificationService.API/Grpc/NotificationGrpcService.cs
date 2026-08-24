using Grpc.Core;
using MediatR;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Notifications.Commands.PushNotification;

namespace NotificationService.API.Grpc;

/// <summary>
/// gRPC service implementation for delivering notifications from other microservices
/// to the <c>NotificationService</c> over the internal gRPC transport.
/// </summary>
public class NotificationGrpcService : NotificationGrpc.NotificationGrpcBase
{
    private readonly IMediator _mediator;
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotificationGrpcService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="NotificationGrpcService"/>.
    /// </summary>
    public NotificationGrpcService(
        IMediator mediator,
        INotificationRepository notificationRepository,
        ILogger<NotificationGrpcService> logger
    )
    {
        _mediator = mediator;
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Receives a push-notification request over gRPC, validates the incoming identifiers,
    /// and dispatches a <see cref="PushNotificationCommand"/> via MediatR.
    /// </summary>
    public override async Task<PushNotificationResponse> PushNotification(
        PushNotificationRequest request,
        ServerCallContext context
    )
    {
        if (!Guid.TryParse(request.UserId, out var userId))
        {
            _logger.LogWarning(
                "PushNotification called with invalid userId: {UserId}",
                request.UserId
            );
            return new PushNotificationResponse { Success = false, NotificationId = string.Empty };
        }

        if (!Guid.TryParse(request.TemplateId, out var templateId))
        {
            _logger.LogWarning(
                "PushNotification called with invalid templateId: {TemplateId}",
                request.TemplateId
            );
            return new PushNotificationResponse { Success = false, NotificationId = string.Empty };
        }

        Guid? visitId = Guid.TryParse(request.VisitId, out var v) ? v : null;
        Guid? complaintId = Guid.TryParse(request.ComplaintId, out var c) ? c : null;
        Guid? amenityBookingId = Guid.TryParse(request.AmenityBookingId, out var a) ? a : null;

        DateTime? scheduledFor = null;
        if (
            !string.IsNullOrWhiteSpace(request.ScheduledFor)
            && DateTime.TryParse(request.ScheduledFor, out var parsedScheduled)
        )
        {
            scheduledFor = DateTime.SpecifyKind(parsedScheduled, DateTimeKind.Utc);
        }

        try
        {
            var result = await _mediator.Send(
                new PushNotificationCommand(
                    userId,
                    templateId,
                    request.Title,
                    request.Message,
                    request.NotificationType,
                    visitId,
                    complaintId,
                    amenityBookingId,
                    string.IsNullOrWhiteSpace(request.RecipientEmail)
                        ? null
                        : request.RecipientEmail,
                    string.IsNullOrWhiteSpace(request.RecipientName) ? null : request.RecipientName,
                    scheduledFor
                )
            );

            return new PushNotificationResponse
            {
                Success = true,
                NotificationId = result.Id.ToString(),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to push notification for user {UserId}", request.UserId);
            return new PushNotificationResponse { Success = false, NotificationId = string.Empty };
        }
    }

    /// <summary>
    /// Looks up the notification template ID for the given notification type and returns it
    /// so calling microservices do not need to hardcode template GUIDs.
    /// </summary>
    public override async Task<GetTemplateIdResponse> GetTemplateIdByType(
        GetTemplateIdRequest request,
        ServerCallContext context
    )
    {
        if (string.IsNullOrWhiteSpace(request.NotificationType))
        {
            _logger.LogWarning("GetTemplateIdByType called with empty notificationType");
            return new GetTemplateIdResponse { Found = false, TemplateId = string.Empty };
        }

        var templateId = await _notificationRepository.GetTemplateIdByTypeAsync(
            request.NotificationType
        );

        if (templateId is null)
        {
            _logger.LogWarning(
                "No notification template found for type {NotificationType}",
                request.NotificationType
            );

            return new GetTemplateIdResponse { Found = false, TemplateId = string.Empty };
        }

        _logger.LogInformation(
            "Template {TemplateId} resolved for type {NotificationType}",
            templateId,
            request.NotificationType
        );

        return new GetTemplateIdResponse { Found = true, TemplateId = templateId.Value.ToString() };
    }

    /// <summary>
    /// Cancels all pending scheduled notifications associated with the given amenity booking.
    /// </summary>
    public override async Task<CancelScheduledNotificationsResponse> CancelScheduledNotifications(
        CancelScheduledNotificationsRequest request,
        ServerCallContext context
    )
    {
        if (!Guid.TryParse(request.AmenityBookingId, out var amenityBookingId))
        {
            _logger.LogWarning(
                "CancelScheduledNotifications called with invalid amenityBookingId: {AmenityBookingId}",
                request.AmenityBookingId
            );
            return new CancelScheduledNotificationsResponse { Success = false, CancelledCount = 0 };
        }

        try
        {
            var count = await _notificationRepository.CancelByAmenityBookingIdAsync(
                amenityBookingId
            );

            _logger.LogInformation(
                "Cancelled {Count} scheduled notifications for booking {BookingId}",
                count,
                amenityBookingId
            );

            return new CancelScheduledNotificationsResponse
            {
                Success = true,
                CancelledCount = count,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to cancel scheduled notifications for booking {BookingId}",
                amenityBookingId
            );
            return new CancelScheduledNotificationsResponse { Success = false, CancelledCount = 0 };
        }
    }
}
