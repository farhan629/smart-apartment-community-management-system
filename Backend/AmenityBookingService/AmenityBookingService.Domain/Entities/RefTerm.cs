using Shared.SharedLibrary.DTO;

namespace AmenityBookingService.Domain.Entities;

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
    /// Gets or sets the collection of amenities associated with the reference term.
    /// </summary>
    public virtual ICollection<Amenity>? Amenities { get; set; }

    /// <summary>
    /// Gets or sets the collection of amenity bookings associated with the reference term.
    /// </summary>
    public virtual ICollection<AmenityBooking>? AmenityBookings { get; set; }
}
