using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Features.Amenities.DTO;
using AmenityBookingService.Application.Settings;
using MediatR;
using Microsoft.Extensions.Options;
using Shared.SharedLibrary.Exceptions;
using Shared.SharedLibrary.Services;

namespace AmenityBookingService.Application.Features.Amenities.Commands;

/// <summary>
/// Command for uploading an amenity image file.
/// </summary>
public class UploadAmenityImageCommand : IRequest<UploadImageResponseDto>
{
    /// <summary>
    /// Gets or sets the original file name of the uploaded image.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the binary content of the uploaded image file.
    /// </summary>
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
}

/// <summary>
/// Handler for processing <see cref="UploadAmenityImageCommand"/> to upload and store an amenity image.
/// </summary>
/// <remarks>
/// Business Rules:
/// - User must be authenticated
/// - Only .jpg, .jpeg, .png, and .svg file types are allowed
/// - File is stored in the configured amenity images directory
/// - File name is generated with a unique GUID to prevent collisions
/// </remarks>
public class UploadAmenityImageHandler
    : IRequestHandler<UploadAmenityImageCommand, UploadImageResponseDto>
{
    private readonly FileStorageSettings _settings;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UploadAmenityImageHandler"/> class.
    /// </summary>
    /// <param name="settings">The file storage configuration settings.</param>
    /// <param name="currentUserService">The current user service for authentication context.</param>
    public UploadAmenityImageHandler(
        IOptions<FileStorageSettings> settings,
        ICurrentUserService currentUserService
    )
    {
        _settings = settings.Value;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Handles the upload and storage of an amenity image file with validation.
    /// </summary>
    /// <param name="request">The upload command containing the file name and binary content.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A response DTO containing the accessible URL of the uploaded image.</returns>
    /// <exception cref="UnauthorizedException">Thrown when user is not authenticated.</exception>
    /// <exception cref="BadRequestException">Thrown when the file type is not supported or extension is invalid.</exception>
    public async Task<UploadImageResponseDto> Handle(
        UploadAmenityImageCommand request,
        CancellationToken cancellationToken
    )
    {
        if (_currentUserService.UserId == Guid.Empty)
            throw new UnauthorizedException(ErrorMessages.UserNotFoundInToken);

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".svg" };
        var ext = Path.GetExtension(request.FileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) || !allowedExtensions.Contains(ext))
            throw new BadRequestException(
                string.Format(
                    ErrorMessages.FileTypeNotSupported,
                    ext,
                    string.Join(", ", allowedExtensions)
                )
            );

        var uploadDir = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), _settings.AmenityImagesPath)
        );
        Directory.CreateDirectory(uploadDir);

        var uniqueFileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadDir, uniqueFileName);

        await File.WriteAllBytesAsync(filePath, request.FileContent, cancellationToken);

        return new UploadImageResponseDto { ImageUrl = $"/uploads/amenities/{uniqueFileName}" };
    }
}
