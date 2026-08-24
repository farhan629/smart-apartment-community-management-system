using Shared.SharedLibrary.DTO;

namespace ComplaintMaintenanceService.Domain.Entities;

/// <summary>
/// Defines rules that auto-assign a complaint to a staff member based on category and priority.
/// </summary>
public class AutoAssignmentRule : BaseEntity
{
    /// <summary>
    /// Gets or sets the category identifier.
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the priority identifier.
    /// </summary>
    /// <remarks>RefTerm FK</remarks>
    public Guid PriorityId { get; set; }

    /// <summary>
    /// Gets or sets the staff identifier.
    /// </summary>
    public Guid StaffId { get; set; }

    /// <summary>
    /// Gets or sets the fallback staff identifier.
    /// </summary>
    /// <remarks>Fallback staff if primary staff is unavailable</remarks>
    public Guid? FallbackStaffId { get; set; }

    /// <summary>
    /// Gets or sets the service duration in minutes.
    /// </summary>
    public int ServiceDurationMinutes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether residents may pick their preferred time slot.
    /// </summary>
    public bool AllowResidentTimePick { get; set; }

    /// <summary>
    /// Gets or sets the time window start.
    /// </summary>
    public TimeSpan? TimeWindowStart { get; set; }

    /// <summary>
    /// Gets or sets the time window end.
    /// </summary>
    public TimeSpan? TimeWindowEnd { get; set; }

    /// <summary>
    /// Gets or sets the category associated with the auto-assignment rule.
    /// </summary>
    public virtual Category? Category { get; set; }

    /// <summary>
    /// Gets or sets the priority reference term.
    /// </summary>
    public virtual RefTerm? Priority { get; set; }

    /// <summary>
    /// Gets or sets the staff member associated with the auto-assignment rule.
    /// </summary>
    public virtual Staff? Staff { get; set; }

    /// <summary>
    /// Gets or sets the fallback staff member associated with the auto-assignment rule.
    /// </summary>
    public virtual Staff? FallbackStaff { get; set; }
}
