using Shared.SharedLibrary.DTO;

namespace NotificationService.Domain.Entities;

/// <summary>
/// Represents a reusable notification template with placeholder variables.
/// </summary>
public class NotificationTemplate : BaseEntity
{
    /// <summary>
    /// Gets or sets the type discriminator for the notification template.
    /// </summary>
    public string NotificationType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of the notification template.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the template body with placeholder variables.
    /// </summary>
    /// <remarks>Supports {{Placeholder}} variables.</remarks>
    public string MessageTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of notifications associated with the template.
    /// </summary>
    public virtual ICollection<Notification>? Notifications { get; set; }
}
