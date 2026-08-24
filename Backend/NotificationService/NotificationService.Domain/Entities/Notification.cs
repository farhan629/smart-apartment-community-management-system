using Shared.SharedLibrary.DTO;

namespace NotificationService.Domain.Entities;

/// <summary>
/// Represents an in-app or push notification sent to a specific user.
/// </summary>
/// <remarks>
/// Cross-service FKs (VisitId, ComplaintId, AmenityBookingId) are stored as plain Guids.
/// No EF navigation since those entities live in other services.
/// </remarks>
public class Notification : BaseEntity
{
    /// <summary>
    /// Gets or sets the template identifier.
    /// </summary>
    public Guid TemplateId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <remarks>Cross-service FK to IdentityService User</remarks>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the visit identifier.
    /// </summary>
    /// <remarks>Cross-service FK to Visit</remarks>
    public Guid? VisitId { get; set; }

    /// <summary>
    /// Gets or sets the complaint identifier.
    /// </summary>
    /// <remarks>Cross-service FK to Complaint</remarks>
    public Guid? ComplaintId { get; set; }

    /// <summary>
    /// Gets or sets the amenity booking identifier.
    /// </summary>
    /// <remarks>Cross-service FK to AmenityBooking</remarks>
    public Guid? AmenityBookingId { get; set; }

    /// <summary>
    /// Gets or sets the title of the notification.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message of the notification.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the delivery status of the notification.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the notification has been read.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Gets or sets the channel discriminator for the notification.
    /// </summary>
    public string NotificationType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the scheduled date and time for the notification.
    /// </summary>
    public DateTime? ScheduledFor { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a reminder has been sent.
    /// </summary>
    public bool IsReminderSent { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the notification was sent.
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the notification was read.
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Gets or sets the notification template associated with the notification.
    /// </summary>
    public virtual NotificationTemplate? Template { get; set; }
}
