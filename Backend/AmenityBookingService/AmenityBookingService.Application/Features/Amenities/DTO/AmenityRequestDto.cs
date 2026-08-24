namespace AmenityBookingService.Application.Features.Amenities.DTO;

/// <summary>
/// Request DTO for creating a new amenity.
/// </summary>
public class CreateAmenityRequestDto
{
    /// <summary>Gets or sets the amenity name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the slot type reference term ID.</summary>
    public Guid SlotTypeId { get; set; }

    /// <summary>Gets or sets the status reference term ID.</summary>
    public Guid StatusId { get; set; }

    /// <summary>Gets or sets the location of the amenity.</summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>Gets or sets the usage rules for the amenity.</summary>
    public string? Rules { get; set; }

    /// <summary>Gets or sets the image URL for the amenity.</summary>
    public string? ImageUrl { get; set; }
}

/// <summary>
/// Request DTO for updating an existing amenity. All properties are optional.
/// </summary>
public class UpdateAmenityRequestDto
{
    /// <summary>Gets or sets the amenity name.</summary>
    public string? Name { get; set; }

    /// <summary>Gets or sets the slot type reference term ID.</summary>
    public Guid? SlotTypeId { get; set; }

    /// <summary>Gets or sets the status reference term ID.</summary>
    public Guid? StatusId { get; set; }

    /// <summary>Gets or sets the location of the amenity.</summary>
    public string? Location { get; set; }

    /// <summary>Gets or sets the usage rules for the amenity.</summary>
    public string? Rules { get; set; }

    /// <summary>Gets or sets the image URL for the amenity.</summary>
    public string? ImageUrl { get; set; }
}
