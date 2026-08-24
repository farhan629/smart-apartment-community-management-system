using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Interfaces.Repositories;
using AmenityBookingService.Domain.Entities;
using AutoMapper;
using MediatR;
using Shared.SharedLibrary.DTO.Common;
using Shared.SharedLibrary.Exceptions;
using Shared.SharedLibrary.Services;

namespace AmenityBookingService.Application.Features.Amenities.Commands;

/// <summary>
/// Command for creating a new amenity.
/// </summary>
public class CreateAmenityCommand : IRequest<IdResponseDto>
{
    /// <summary>
    /// Gets or sets the name of the amenity.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the slot type identifier for the amenity.
    /// </summary>
    public Guid SlotTypeId { get; set; }

    /// <summary>
    /// Gets or sets the status identifier for the amenity.
    /// </summary>
    public Guid StatusId { get; set; }

    /// <summary>
    /// Gets or sets the location of the amenity.
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the usage rules for the amenity.
    /// </summary>
    public string? Rules { get; set; }

    /// <summary>
    /// Gets or sets the image URL for the amenity.
    /// </summary>
    public string? ImageUrl { get; set; }
}

/// <summary>
/// Handler for processing <see cref="CreateAmenityCommand"/> to create a new amenity.
/// </summary>
/// <remarks>
/// Business Rules:
/// - Amenity name must be unique
/// - SlotType and Status must exist in reference data
/// - User must be authenticated
/// </remarks>
public class CreateAmenityHandler : IRequestHandler<CreateAmenityCommand, IdResponseDto>
{
    private readonly IAmenityRepository _repository;
    private readonly IRefTermRepository _refTermRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreateAmenityHandler(
        IAmenityRepository repository,
        IRefTermRepository refTermRepository,
        IMapper mapper,
        ICurrentUserService currentUserService
    )
    {
        _repository = repository;
        _refTermRepository = refTermRepository;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<IdResponseDto> Handle(
        CreateAmenityCommand request,
        CancellationToken cancellationToken
    )
    {
        if (_currentUserService.UserId == Guid.Empty)
            throw new UnauthorizedException(ErrorMessages.UserNotFoundInToken);

        var exists = await _repository.ExistsByNameAsync(request.Name, cancellationToken);
        if (exists)
            throw new ConflictException(
                string.Format(ErrorMessages.AmenityNameAlreadyExists, request.Name)
            );

        var slotType = await _refTermRepository.GetRefTermByIdAsync(
            request.SlotTypeId,
            cancellationToken
        );
        if (slotType == null)
            throw new NotFoundException(
                string.Format(ErrorMessages.SlotTypeNotFound, request.SlotTypeId)
            );

        var status = await _refTermRepository.GetRefTermByIdAsync(
            request.StatusId,
            cancellationToken
        );
        if (status == null)
            throw new NotFoundException(
                string.Format(ErrorMessages.StatusNotFound, request.StatusId)
            );

        var amenity = _mapper.Map<Amenity>(request);

        var result = await _repository.CreateAsync(amenity, cancellationToken);

        return new IdResponseDto { Id = result.Id };
    }
}
