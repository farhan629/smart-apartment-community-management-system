namespace ResidentVisitorService.Application.Features.Visitors.Services;

/// <summary>
/// Defines file storage operations for visitor-related uploads.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Saves an uploaded photo file to the storage location and returns the relative URL.
    /// </summary>
    Task<string> SaveVisitorPhotoAsync(
        Guid visitorId,
        Stream fileStream,
        string extension,
        CancellationToken cancellationToken = default
    );

    Task<string> SaveAsync(
        Stream stream,
        string folder,
        string fileName,
        CancellationToken cancellationToken = default
    );
}
