namespace AmenityBookingService.Application.Interfaces.Services;

/// <summary>
/// Defines methods for communicating with the Notification service.
/// </summary>
public interface INotificationClient
{
    /// <summary>
    /// Sends a notification to the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the recipient.</param>
    /// <param name="title">The notification title.</param>
    /// <param name="message">The notification message.</param>
    /// <param name="notificationType">The type of notification.</param>
    /// <param name="amenityBookingId">The associated amenity booking identifier, if applicable.</param>
    /// <param name="scheduledFor">The scheduled delivery date and time, if applicable.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    Task SendNotificationAsync(
        Guid userId,
        string title,
        string message,
        string notificationType,
        Guid? amenityBookingId = null,
        DateTime? scheduledFor = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Cancels all pending scheduled notifications associated with the given amenity booking.
    /// </summary>
    /// <param name="amenityBookingId">The booking whose scheduled notifications should be cancelled.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// /// <returns>The number of notifications that were cancelled.</returns>
    Task<int> CancelScheduledNotificationsAsync(
        Guid amenityBookingId,
        CancellationToken cancellationToken = default
    );
}
