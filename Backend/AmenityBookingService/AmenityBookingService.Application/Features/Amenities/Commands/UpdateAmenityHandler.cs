using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Interfaces.Repositories;
using MediatR;
using Shared.SharedLibrary.DTO.Common;
using Shared.SharedLibrary.Exceptions;
using Shared.SharedLibrary.Services;

namespace AmenityBookingService.Application.Features.Amenities.Commands;

/// <summary>
/// Command for updating an existing amenity's details.
/// </summary>
/// <remarks>
/// All properties are optional. Only provided fields will be updated.
/// </remarks>
public class UpdateAmenityCommand : IRequest<MessageResponseDto>
{
    /// <summary>
    /// Gets or sets the unique identifier of the amenity to update.
    /// </summary>
    public Guid AmenityId { get; set; }

    /// <summary>
    /// Gets or sets the updated name of the amenity. Optional.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the updated slot type identifier. Optional.
    /// </summary>
    public Guid? SlotTypeId { get; set; }

    /// <summary>
    /// Gets or sets the updated status identifier. Optional.
    /// </summary>
    public Guid? StatusId { get; set; }

    /// <summary>
    /// Gets or sets the updated location of the amenity. Optional.
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Gets or sets the updated usage rules. Optional.
    /// </summary>
    public string? Rules { get; set; }

    /// <summary>
    /// Gets or sets the updated image URL. Optional.
    /// </summary>
    public string? ImageUrl { get; set; }
}

/// <summary>
/// Handler for processing <see cref="UpdateAmenityCommand"/> to update an existing amenity.
/// </summary>
/// <remarks>
/// Business Rules:
/// - Amenity must exist
/// - Only provided fields are updated (partial update)
/// - Name must be unique if being changed
/// - SlotType and Status must exist in reference data if being changed
/// - User must be authenticated
/// </remarks>
public class UpdateAmenityHandler : IRequestHandler<UpdateAmenityCommand, MessageResponseDto>
{
    private readonly IAmenityRepository _repository;
    private readonly IRefTermRepository _refTermRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateAmenityHandler(
        IAmenityRepository repository,
        IRefTermRepository refTermRepository,
        ICurrentUserService currentUserService
    )
    {
        _repository = repository;
        _refTermRepository = refTermRepository;
        _currentUserService = currentUserService;
    }

    public async Task<MessageResponseDto> Handle(
        UpdateAmenityCommand request,
        CancellationToken cancellationToken
    )
    {
        if (_currentUserService.UserId == Guid.Empty)
            throw new UnauthorizedException(ErrorMessages.UserNotFoundInToken);

        var amenity = await _repository.GetByIdAsync(request.AmenityId, cancellationToken);
        if (amenity == null)
            throw new NotFoundException(
                string.Format(ErrorMessages.AmenityNotFound, request.AmenityId)
            );

        if (!string.IsNullOrEmpty(request.Name) && request.Name != amenity.Name)
        {
            var exists = await _repository.ExistsByNameAsync(
                request.Name,
                request.AmenityId,
                cancellationToken
            );
            if (exists)
                throw new ConflictException(
                    string.Format(ErrorMessages.AmenityNameAlreadyExists, request.Name)
                );
            amenity.Name = request.Name;
        }

        if (request.SlotTypeId.HasValue)
        {
            var slotType = await _refTermRepository.GetRefTermByIdAsync(
                request.SlotTypeId.Value,
                cancellationToken
            );
            if (slotType == null)
                throw new NotFoundException(
                    string.Format(ErrorMessages.SlotTypeNotFound, request.SlotTypeId)
                );
            amenity.SlotTypeId = request.SlotTypeId.Value;
        }

        if (request.StatusId.HasValue)
        {
            var status = await _refTermRepository.GetRefTermByIdAsync(
                request.StatusId.Value,
                cancellationToken
            );
            if (status == null)
                throw new NotFoundException(
                    string.Format(ErrorMessages.StatusNotFound, request.StatusId)
                );
            amenity.StatusId = request.StatusId.Value;
        }

        if (!string.IsNullOrEmpty(request.Location))
            amenity.Location = request.Location;

        if (request.Rules != null)
            amenity.Rules = request.Rules;

        if (request.ImageUrl != null)
            amenity.ImageUrl = request.ImageUrl;

        await _repository.UpdateAsync(amenity, cancellationToken);

        return new MessageResponseDto { Message = SuccessMessages.UpdatedMessage };
    }
}
