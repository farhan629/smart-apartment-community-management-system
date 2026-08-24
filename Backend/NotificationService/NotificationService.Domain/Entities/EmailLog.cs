using Shared.SharedLibrary.DTO;

namespace NotificationService.Domain.Entities;

/// <summary>
/// Represents an audit log of every email dispatched by the system.
/// </summary>
public class EmailLog : BaseEntity
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
    /// Gets or sets the email address of the recipient.
    /// </summary>
    public string EmailAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subject of the email.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the body of the email.
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status of the email.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message if the email failed to send.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the email was sent.
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Gets or sets the email template associated with the log.
    /// </summary>
    public virtual EmailTemplate? Template { get; set; }
}
