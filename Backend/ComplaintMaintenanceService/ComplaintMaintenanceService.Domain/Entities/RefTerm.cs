using Shared.SharedLibrary.DTO;

namespace ComplaintMaintenanceService.Domain.Entities;

/// <summary>
/// Represents a reference term within a reference set.
/// </summary>
public class RefTerm : BaseEntity
{
    /// <summary>
    /// Gets or sets the reference set identifier.
    /// </summary>
    public Guid RefSetId { get; set; }

    /// <summary>
    /// Gets or sets the code of the reference term.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the reference term.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reference set associated with the reference term.
    /// </summary>
    public virtual RefSet? RefSet { get; set; }

    /// <summary>
    /// Gets or sets the collection of complaints associated with the reference term.
    /// </summary>
    public virtual ICollection<Complaint>? Complaints { get; set; }

    /// <summary>
    /// Gets or sets the collection of complaint assignments associated with the reference term.
    /// </summary>
    public virtual ICollection<ComplaintAssignment>? ComplaintAssignments { get; set; }

    /// <summary>
    /// Gets or sets the collection of complaint progress logs associated with the reference term.
    /// </summary>
    public virtual ICollection<ComplaintProgressLog>? ComplaintProgressLogs { get; set; }

    /// <summary>
    /// Gets or sets the collection of auto-assignment rules associated with the reference term.
    /// </summary>
    public virtual ICollection<AutoAssignmentRule>? AutoAssignmentRules { get; set; }
}
