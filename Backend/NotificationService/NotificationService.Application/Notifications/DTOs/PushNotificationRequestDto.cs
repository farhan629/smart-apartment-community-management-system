using NotificationService.Application.Constants;

namespace NotificationService.Application.Notifications.DTOs;

/// <summary>
/// Carries all data required to create an in-app notification and optionally
/// trigger a transactional email for the recipient.
/// </summary>
public class PushNotificationRequestDto
{
    /// <summary>The unique identifier of the user who will receive the notification.</summary>
    public Guid UserId { get; set; }

    /// <summary>The unique identifier of the notification template to associate with this notification.</summary>
    public Guid TemplateId { get; set; }

    /// <summary>The notification title displayed in the UI.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>The notification body text.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// A string code classifying the notification (e.g. <c>"visitor_approved"</c>).
    /// Defaults to <see cref="NotificationConstants.DEFAULT_NOTIFICATION_TYPE"/> when not supplied.
    /// </summary>
    public string NotificationType { get; set; } = NotificationConstants.DEFAULT_NOTIFICATION_TYPE;

    /// <summary>Optional reference to a related visitor management record.</summary>
    public Guid? VisitId { get; set; }

    /// <summary>Optional reference to a related complaint record.</summary>
    public Guid? ComplaintId { get; set; }

    /// <summary>Optional reference to a related amenity booking record.</summary>
    public Guid? AmenityBookingId { get; set; }

    /// <summary>
    /// Optional recipient email address. When provided, a transactional email is dispatched
    /// alongside the in-app notification via <c>SendEmailCommand</c>.
    /// </summary>
    public string? RecipientEmail { get; set; }

    /// <summary>
    /// Optional recipient display name used for email personalisation placeholders.
    /// Ignored when <see cref="RecipientEmail"/> is not supplied.
    /// </summary>
    public string? RecipientName { get; set; }
}
