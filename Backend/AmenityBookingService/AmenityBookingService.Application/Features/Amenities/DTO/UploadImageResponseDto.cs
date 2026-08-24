namespace AmenityBookingService.Application.Features.Amenities.DTO;

/// <summary>
/// Response DTO returned after a successful amenity image upload.
/// </summary>
public class UploadImageResponseDto
{
    /// <summary>Gets or sets the accessible URL of the uploaded image.</summary>
    public string ImageUrl { get; set; } = string.Empty;
}
