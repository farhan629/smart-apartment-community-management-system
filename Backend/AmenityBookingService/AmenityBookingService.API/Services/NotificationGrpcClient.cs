using AmenityBookingService.Application.Interfaces.Services;
using NotificationService.API.Grpc;

namespace AmenityBookingService.API.Services;

/// <summary>
/// Provides a gRPC client implementation for communicating with the
/// Notification Service to send notifications to users.
/// </summary>
public class NotificationGrpcClient : INotificationClient
{
    private readonly NotificationGrpc.NotificationGrpcClient _client;
    private readonly ILogger<NotificationGrpcClient> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationGrpcClient"/> class.
    /// </summary>
    /// <param name="client">
    /// The gRPC client used to communicate with the Notification Service.
    /// </param>
    /// <param name="logger">
    /// The logger used to record service operations and errors.
    /// </param>
    public NotificationGrpcClient(
        NotificationGrpc.NotificationGrpcClient client,
        ILogger<NotificationGrpcClient> logger
    )
    {
        _client = client;
        _logger = logger;
    }

    /// <summary>
    /// Sends a notification to the specified user through the Notification Service.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user who will receive the notification.
    /// </param>
    /// <param name="title">
    /// The title of the notification.
    /// </param>
    /// <param name="message">
    /// The content of the notification.
    /// </param>
    /// <param name="notificationType">
    /// The type of notification used to determine the appropriate notification template.
    /// </param>
    /// <param name="amenityBookingId">
    /// The unique identifier of the related amenity booking, if applicable.
    /// </param>
    /// <param name="scheduledFor">
    /// The date and time at which the notification should be delivered, if scheduled.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous notification operation.
    /// </returns>
    public async Task SendNotificationAsync(
        Guid userId,
        string title,
        string message,
        string notificationType,
        Guid? amenityBookingId = null,
        DateTime? scheduledFor = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var templateId = Guid.Empty;

            try
            {
                var templateResponse = await _client.GetTemplateIdByTypeAsync(
                    new GetTemplateIdRequest { NotificationType = notificationType },
                    cancellationToken: cancellationToken
                );

                if (
                    templateResponse.Found
                    && Guid.TryParse(templateResponse.TemplateId, out var tid)
                )
                {
                    templateId = tid;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Failed to resolve template for type {Type}, using fallback",
                    notificationType
                );
            }

            _logger.LogInformation(
                "[NotificationGrpcClient] Pushing notification type={Type}, userId={UserId}, scheduledFor={ScheduledFor}",
                notificationType,
                userId,
                scheduledFor?.ToString("o") ?? "null"
            );

            var request = new PushNotificationRequest
            {
                UserId = userId.ToString(),
                TemplateId = templateId.ToString(),
                Title = title,
                Message = message,
                NotificationType = notificationType,
                AmenityBookingId = amenityBookingId?.ToString() ?? string.Empty,
                ScheduledFor = scheduledFor?.ToString("o") ?? string.Empty,
            };

            var response = await _client.PushNotificationAsync(
                request,
                cancellationToken: cancellationToken
            );

            if (!response.Success)
            {
                _logger.LogWarning(
                    "Failed to push notification for user {UserId}, type {Type}",
                    userId,
                    notificationType
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error sending notification for user {UserId}, type {Type}",
                userId,
                notificationType
            );
        }
    }

    /// <summary>
    /// Cancels all pending scheduled notifications associated with the given amenity booking.
    /// </summary>
    public async Task<int> CancelScheduledNotificationsAsync(
        Guid amenityBookingId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var response = await _client.CancelScheduledNotificationsAsync(
                new CancelScheduledNotificationsRequest
                {
                    AmenityBookingId = amenityBookingId.ToString(),
                },
                cancellationToken: cancellationToken
            );

            if (response.Success)
            {
                _logger.LogInformation(
                    "Cancelled {Count} scheduled notifications for booking {BookingId}",
                    response.CancelledCount,
                    amenityBookingId
                );
            }

            return response.CancelledCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error cancelling scheduled notifications for booking {BookingId}",
                amenityBookingId
            );
            return 0;
        }
    }
}
