using Shared.SharedLibrary.DTO;

namespace AmenityBookingService.Domain.Entities;

/// <summary>
/// Represents a specific bookable time slot for an amenity.
/// </summary>
public class AmenitySlot : BaseEntity
{
    /// <summary>
    /// Gets or sets the amenity identifier.
    /// </summary>
    public Guid AmenityId { get; set; }

    /// <summary>
    /// Gets or sets the human-readable label for the slot.
    /// </summary>
    public string SlotLabel { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of the slot.
    /// </summary>
    public DateTime SlotDate { get; set; }

    /// <summary>
    /// Gets or sets the start time of the slot.
    /// </summary>
    public TimeSpan StartTime { get; set; }

    /// <summary>
    /// Gets or sets the end time of the slot.
    /// </summary>
    public TimeSpan EndTime { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of people allowed for the slot.
    /// </summary>
    public int MaxCapacity { get; set; }

    /// <summary>
    /// Gets or sets the current running count of confirmed bookings.
    /// </summary>
    public int CurrentBookingCount { get; set; }

    /// <summary>
    /// Gets or sets the amenity associated with the slot.
    /// </summary>
    public virtual Amenity? Amenity { get; set; }

    /// <summary>
    /// Gets or sets the collection of amenity bookings associated with the slot.
    /// </summary>
    public virtual ICollection<AmenityBooking>? AmenityBookings { get; set; }
}
