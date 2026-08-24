namespace AmenityBookingService.Application.Settings;

/// <summary>
/// Configuration settings for file storage paths, bound from the "FileStorage" section in appsettings.json.
/// </summary>
public class FileStorageSettings
{
    /// <summary>The configuration section name used to bind these settings.</summary>
    public const string SectionName = "FileStorage";
    /// <summary>Gets or sets the relative path for storing amenity images. Default: "wwwroot/uploads/amenities".</summary>
    public string AmenityImagesPath { get; set; } = "wwwroot/uploads/amenities";
}
