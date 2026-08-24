using Shared.SharedLibrary.DTO;

namespace AmenityBookingService.Domain.Entities;

/// <summary>
/// Represents a booking made by a resident for an amenity slot.
/// </summary>
public class AmenityBooking : BaseEntity
{
    /// <summary>
    /// Gets or sets the amenity slot identifier.
    /// </summary>
    public Guid AmenitySlotId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <remarks>Cross-service FK to IdentityService User</remarks>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the booking status identifier.
    /// </summary>
    /// <remarks>RefTerm FK</remarks>
    public Guid BookingStatusId { get; set; }

    /// <summary>
    /// Gets or sets the number of people included in this booking.
    /// </summary>
    public int PeopleCount { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the booking was cancelled.
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Gets or sets the reason for cancellation.
    /// </summary>
    /// <remarks>Only set when cancelled</remarks>
    public string? CancellationReason { get; set; }

    /// <summary>
    /// Gets or sets the amenity slot associated with the booking.
    /// </summary>
    public virtual AmenitySlot? AmenitySlot { get; set; }

    /// <summary>
    /// Gets or sets the booking status reference term.
    /// </summary>
    public virtual RefTerm? BookingStatus { get; set; }
}
