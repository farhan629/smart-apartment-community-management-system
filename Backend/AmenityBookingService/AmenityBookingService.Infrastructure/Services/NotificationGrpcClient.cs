using AmenityBookingService.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using NotificationService.API.Grpc;

namespace AmenityBookingService.Infrastructure.Services;

/// <summary>
/// Provides gRPC client implementation for communicating with the Notification Service.
/// </summary>
/// <remarks>
/// This class implements the <see cref="INotificationClient"/> interface and uses gRPC
/// to interact with the Notification Service for sending and managing notifications.
/// </remarks>
public class NotificationGrpcClient : INotificationClient
{
    private readonly NotificationGrpc.NotificationGrpcClient _client;
    private readonly ILogger<NotificationGrpcClient> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationGrpcClient"/> class.
    /// </summary>
    /// <param name="client">The gRPC client for the Notification Service.</param>
    /// <param name="logger">The logger instance for recording operations and errors.</param>
    public NotificationGrpcClient(
        NotificationGrpc.NotificationGrpcClient client,
        ILogger<NotificationGrpcClient> logger
    )
    {
        _client = client;
        _logger = logger;
    }

    /// <summary>
    /// Sends a notification to a specified user via the Notification Service.
    /// </summary>
    /// <param name="userId">The unique identifier of the target user.</param>
    /// <param name="title">The title of the notification.</param>
    /// <param name="message">The content/message body of the notification.</param>
    /// <param name="notificationType">The type/category of the notification (e.g., "BookingConfirmation", "Reminder").</param>
    /// <param name="amenityBookingId">Optional identifier for the associated amenity booking.</param>
    /// <param name="scheduledFor">Optional date and time when the notification should be sent. If null, sends immediately.</param>
    /// <param name="cancellationToken">Cancellation token for the operation. Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method attempts to resolve a template ID based on the notification type. If template resolution fails,
    /// a warning is logged and a fallback template ID (empty GUID) is used. The notification is then pushed via the
    /// Notification Service gRPC endpoint. Any errors during the process are logged but not thrown to maintain service resilience.
    /// </remarks>
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
    /// Cancels all scheduled notifications associated with a specific amenity booking.
    /// </summary>
    /// <param name="amenityBookingId">The unique identifier of the amenity booking.</param>
    /// <param name="cancellationToken">Cancellation token for the operation. Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <returns>
    /// The number of cancelled notifications. Returns 0 if an error occurs or no scheduled notifications exist.
    /// </returns>
    /// <remarks>
    /// This method makes a gRPC call to the Notification Service to cancel all scheduled notifications
    /// for the specified booking. Successful cancellations are logged with the count, while errors are logged
    /// and a value of 0 is returned to prevent service disruption.
    /// </remarks>
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
