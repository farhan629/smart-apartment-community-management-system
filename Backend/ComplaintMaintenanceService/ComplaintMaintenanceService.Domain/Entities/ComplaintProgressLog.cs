using Shared.SharedLibrary.DTO;

namespace ComplaintMaintenanceService.Domain.Entities;

/// <summary>
/// Represents an audit log of status changes on a complaint.
/// </summary>
public class ComplaintProgressLog : BaseEntity
{
    /// <summary>
    /// Gets or sets the complaint identifier.
    /// </summary>
    public Guid ComplaintId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier who changed the status.
    /// </summary>
    /// <remarks>Cross-service FK to IdentityService User</remarks>
    public Guid ChangedBy { get; set; }

    /// <summary>
    /// Gets or sets the status identifier for the new status at the time of this log entry.
    /// </summary>
    /// <remarks>RefTerm FK</remarks>
    public Guid StatusId { get; set; }

    /// <summary>
    /// Gets or sets the remarks for the status change.
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the status was changed.
    /// </summary>
    public DateTime ChangedDate { get; set; }

    /// <summary>
    /// Gets or sets the complaint associated with the progress log.
    /// </summary>
    public virtual Complaint? Complaint { get; set; }

    /// <summary>
    /// Gets or sets the status reference term.
    /// </summary>
    public virtual RefTerm? Status { get; set; }
}
