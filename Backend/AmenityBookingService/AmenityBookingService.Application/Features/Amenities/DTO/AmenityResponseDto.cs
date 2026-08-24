using Shared.SharedLibrary.DTO.Common;

namespace AmenityBookingService.Application.Features.Amenities.DTO;

/// <summary>
/// Response DTO representing a single amenity.
/// </summary>
public class AmenityResponseDto
{
    /// <summary>Gets or sets the amenity ID.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the amenity name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the slot type code.</summary>
    public string SlotType { get; set; } = string.Empty;

    /// <summary>Gets or sets the status code.</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Gets or sets the location of the amenity.</summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>Gets or sets the usage rules for the amenity.</summary>
    public string Rules { get; set; } = string.Empty;

    /// <summary>Gets or sets the image URL for the amenity.</summary>
    public string ImageUrl { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO containing a paginated list of amenities.
/// </summary>
public class AmenityListResponseDto
{
    /// <summary>Gets or sets the list of amenity response DTOs.</summary>
    public List<AmenityResponseDto> Data { get; set; } = new();

    /// <summary>Gets or sets the pagination metadata.</summary>
    public PaginationDto Pagination { get; set; } = new();
}
