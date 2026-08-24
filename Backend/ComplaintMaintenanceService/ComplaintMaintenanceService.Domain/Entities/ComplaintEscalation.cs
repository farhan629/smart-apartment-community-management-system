using Shared.SharedLibrary.DTO;

namespace ComplaintMaintenanceService.Domain.Entities;

/// <summary>
/// Represents an escalation of a complaint to a higher authority.
/// </summary>
public class ComplaintEscalation : BaseEntity
{
    /// <summary>
    /// Gets or sets the complaint identifier.
    /// </summary>
    public Guid ComplaintId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier who raised the escalation.
    /// </summary>
    /// <remarks>Cross-service FK to IdentityService User</remarks>
    public Guid EscalatedBy { get; set; }

    /// <summary>
    /// Gets or sets the user identifier to whom the complaint was escalated.
    /// </summary>
    /// <remarks>Cross-service FK to IdentityService User</remarks>
    public Guid EscalatedTo { get; set; }

    /// <summary>
    /// Gets or sets the reason for escalation.
    /// </summary>
    public string EscalationReason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the escalation was raised.
    /// </summary>
    public DateTime EscalationDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the complaint was resolved after escalation.
    /// </summary>
    public bool ResolvedAfterEscalation { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the escalation was resolved.
    /// </summary>
    public DateTime? ResolutionDate { get; set; }

    /// <summary>
    /// Gets or sets the complaint associated with the escalation.
    /// </summary>
    public virtual Complaint? Complaint { get; set; }
}
