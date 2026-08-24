using Shared.SharedLibrary.DTO;

namespace NotificationService.Domain.Entities;

/// <summary>
/// Represents a reusable email template with subject and HTML body placeholders.
/// </summary>
public class EmailTemplate : BaseEntity
{
    /// <summary>
    /// Gets or sets the type discriminator for the email template.
    /// </summary>
    public string EmailType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subject of the email template.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HTML body template with placeholder variables.
    /// </summary>
    /// <remarks>Supports {{Placeholder}} variables.</remarks>
    public string BodyTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of email logs associated with the template.
    /// </summary>
    public virtual ICollection<EmailLog>? EmailLogs { get; set; }
}
