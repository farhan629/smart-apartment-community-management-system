using Shared.SharedLibrary.DTO;

namespace AmenityBookingService.Domain.Entities;

/// <summary>
/// Represents an amenity available for booking in the smart apartment community.
/// </summary>
public class Amenity : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the amenity.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the slot type identifier for the amenity.
    /// </summary>
    /// <remarks>RefTerm FK</remarks>
    public Guid SlotTypeId { get; set; }

    /// <summary>
    /// Gets or sets the status identifier of the amenity.
    /// </summary>
    /// <remarks>RefTerm FK</remarks>
    public Guid StatusId { get; set; }

    /// <summary>
    /// Gets or sets the physical location of the amenity.
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rules and regulations for using the amenity.
    /// </summary>
    public string Rules { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the image URL of the amenity.
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the slot type reference term.
    /// </summary>
    public virtual RefTerm? SlotType { get; set; }

    /// <summary>
    /// Gets or sets the status reference term.
    /// </summary>
    public virtual RefTerm? Status { get; set; }

    /// <summary>
    /// Gets or sets the collection of amenity slots associated with the amenity.
    /// </summary>
    public virtual ICollection<AmenitySlot>? AmenitySlots { get; set; }
}
