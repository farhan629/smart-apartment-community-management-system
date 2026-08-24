using Shared.SharedLibrary.DTO;

namespace ComplaintMaintenanceService.Domain.Entities;

/// <summary>
/// Represents a staff member who handles maintenance complaints.
/// </summary>
public class Staff : BaseEntity
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <remarks>Cross-service FK to IdentityService User. One User maps to one Staff profile (1:1).</remarks>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the category identifier.
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the description of the staff member.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the details of the staff member.
    /// </summary>
    public string Details { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category associated with the staff member.
    /// </summary>
    public virtual Category? Category { get; set; }

    /// <summary>
    /// Gets or sets the collection of staff availabilities associated with the staff member.
    /// </summary>
    public virtual ICollection<StaffAvailability>? StaffAvailabilities { get; set; }

    /// <summary>
    /// Gets or sets the collection of complaint assignments associated with the staff member.
    /// </summary>
    public virtual ICollection<ComplaintAssignment>? ComplaintAssignments { get; set; }

    /// <summary>
    /// Gets or sets the collection of auto-assignment rules associated with the staff member.
    /// </summary>
    public virtual ICollection<AutoAssignmentRule>? AutoAssignmentRules { get; set; }
}
