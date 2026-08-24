using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visitors.DTOs;
using ResidentVisitorService.Application.Features.Visitors.Services;
using ResidentVisitorService.Application.Interfaces.Repositories;

namespace ResidentVisitorService.Application.Features.Visitors.Commands;

/// <summary>Command to upload or replace a visitor's profile photo.</summary>
public class UploadVisitorPhotoCommand : IRequest<VisitorResponseDto>
{
    /// <summary>Gets or sets the visitor's unique identifier.</summary>
    public Guid VisitorId { get; set; }

    /// <summary>Gets or sets the uploaded photo file.</summary>
    public IFormFile Photo { get; set; } = null!;
}

/// <summary>Handles the <see cref="UploadVisitorPhotoCommand"/> request.</summary>
public class UploadVisitorPhotoCommandHandler
    : IRequestHandler<UploadVisitorPhotoCommand, VisitorResponseDto>
{
    private readonly IVisitorRepository _visitorRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;
    private readonly ILogger<UploadVisitorPhotoCommandHandler> _logger;

    public UploadVisitorPhotoCommandHandler(
        IVisitorRepository visitorRepository,
        IFileStorageService fileStorageService,
        IMapper mapper,
        ILogger<UploadVisitorPhotoCommandHandler> logger
    )
    {
        _visitorRepository = visitorRepository;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<VisitorResponseDto> Handle(
        UploadVisitorPhotoCommand command,
        CancellationToken cancellationToken
    )
    {
        var visitor =
            await _visitorRepository.GetByIdAsync(command.VisitorId, cancellationToken)
            ?? throw new KeyNotFoundException(
                string.Format(ResidentVisitorConstants.Errors.VisitorNotFound, command.VisitorId)
            );

        var photo = command.Photo;

        if (photo.Length == 0)
        {
            throw new InvalidOperationException(ResidentVisitorConstants.Errors.PhotoFileEmpty);
        }

        if (photo.Length > ResidentVisitorConstants.PhotoUpload.MaxFileSizeBytes)
        {
            throw new InvalidOperationException(ResidentVisitorConstants.Errors.PhotoFileTooLarge);
        }

        var extension = Path.GetExtension(photo.FileName);
        if (!ResidentVisitorConstants.PhotoUpload.AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException(
                string.Format(
                    ResidentVisitorConstants.Errors.PhotoInvalidExtension,
                    extension,
                    string.Join(", ", ResidentVisitorConstants.PhotoUpload.AllowedExtensions)
                )
            );
        }

        _logger.LogInformation(
            "Uploading photo for visitor {VisitorId} — file: {FileName}, size: {Size} bytes",
            command.VisitorId,
            photo.FileName,
            photo.Length
        );

        await using var stream = photo.OpenReadStream();
        var photoUrl = await _fileStorageService.SaveVisitorPhotoAsync(
            command.VisitorId,
            stream,
            extension,
            cancellationToken
        );

        await _visitorRepository.UpdatePhotoUrlAsync(
            command.VisitorId,
            photoUrl,
            cancellationToken
        );

        _logger.LogInformation(
            "Photo uploaded successfully for visitor {VisitorId} — url: {PhotoUrl}",
            command.VisitorId,
            photoUrl
        );

        visitor.PhotoUrl = photoUrl;

        return _mapper.Map<VisitorResponseDto>(visitor);
    }
}
