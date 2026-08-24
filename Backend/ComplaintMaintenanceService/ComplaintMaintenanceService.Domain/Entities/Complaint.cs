using Shared.SharedLibrary.DTO;

namespace ComplaintMaintenanceService.Domain.Entities;

/// <summary>
/// Represents a complaint raised by a resident.
/// </summary>
public class Complaint : BaseEntity
{
    /// <summary>
    /// Gets or sets the resident identifier.
    /// </summary>
    /// <remarks>Cross-service FK to IdentityService User</remarks>
    public Guid ResidentId { get; set; }

    /// <summary>
    /// Gets or sets the complaint type identifier.
    /// </summary>
    /// <remarks>RefTerm FK</remarks>
    public Guid ComplaintTypeId { get; set; }

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
    /// Gets or sets the status identifier.
    /// </summary>
    /// <remarks>RefTerm FK</remarks>
    public Guid StatusId { get; set; }

    /// <summary>
    /// Gets or sets the scheduled slot identifier.
    /// </summary>
    /// <remarks>FK to StaffAvailability slot chosen for this complaint</remarks>
    public Guid? ScheduledSlotId { get; set; }

    /// <summary>
    /// Gets or sets the description of the complaint.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the scheduled date of the complaint.
    /// </summary>
    public DateTime? ScheduledDate { get; set; }

    /// <summary>
    /// Gets or sets the scheduled time of the complaint.
    /// </summary>
    public TimeSpan? ScheduledTime { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the complaint was cancelled.
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Gets or sets the reason for cancellation.
    /// </summary>
    public string? CancellationReason { get; set; }

    /// <summary>
    /// Gets or sets the complaint type reference term.
    /// </summary>
    public virtual RefTerm? ComplaintType { get; set; }

    /// <summary>
    /// Gets or sets the category associated with the complaint.
    /// </summary>
    public virtual Category? Category { get; set; }

    /// <summary>
    /// Gets or sets the priority reference term.
    /// </summary>
    public virtual RefTerm? Priority { get; set; }

    /// <summary>
    /// Gets or sets the status reference term.
    /// </summary>
    public virtual RefTerm? Status { get; set; }

    /// <summary>
    /// Gets or sets the scheduled slot associated with the complaint.
    /// </summary>
    public virtual StaffAvailability? ScheduledSlot { get; set; }

    /// <summary>
    /// Gets or sets the collection of complaint assignments associated with the complaint.
    /// </summary>
    public virtual ICollection<ComplaintAssignment>? ComplaintAssignments { get; set; }

    /// <summary>
    /// Gets or sets the collection of complaint progress logs associated with the complaint.
    /// </summary>
    public virtual ICollection<ComplaintProgressLog>? ComplaintProgressLogs { get; set; }

    /// <summary>
    /// Gets or sets the collection of complaint comments associated with the complaint.
    /// </summary>
    public virtual ICollection<ComplaintComment>? ComplaintComments { get; set; }

    /// <summary>
    /// Gets or sets the collection of complaint escalations associated with the complaint.
    /// </summary>
    public virtual ICollection<ComplaintEscalation>? ComplaintEscalations { get; set; }
}
