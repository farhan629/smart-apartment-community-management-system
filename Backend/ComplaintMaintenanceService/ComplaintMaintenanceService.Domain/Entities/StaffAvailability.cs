using Shared.SharedLibrary.DTO;

namespace ComplaintMaintenanceService.Domain.Entities;

/// <summary>
/// Represents an availability slot for a staff member.
/// </summary>
public class StaffAvailability : BaseEntity
{
    /// <summary>
    /// Gets or sets the staff identifier.
    /// </summary>
    public Guid StaffId { get; set; }

    /// <summary>
    /// Gets or sets the complaint identifier.
    /// </summary>
    /// <remarks>Set once this slot is booked against a complaint</remarks>
    public Guid? ComplaintId { get; set; }

    /// <summary>
    /// Gets or sets the available date of the slot.
    /// </summary>
    public DateTime AvailableDate { get; set; }

    /// <summary>
    /// Gets or sets the slot start time.
    /// </summary>
    public TimeSpan SlotStartTime { get; set; }

    /// <summary>
    /// Gets or sets the slot end time.
    /// </summary>
    public TimeSpan SlotEndTime { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the slot is booked.
    /// </summary>
    public bool IsBooked { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the slot is cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Gets or sets the staff member associated with the availability slot.
    /// </summary>
    public virtual Staff? Staff { get; set; }

    /// <summary>
    /// Gets or sets the complaint associated with the availability slot.
    /// </summary>
    public virtual Complaint? Complaint { get; set; }
}
