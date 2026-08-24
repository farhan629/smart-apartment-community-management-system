using AmenityBookingService.Application.Constants;
using AmenityBookingService.Application.Features.Amenities.DTO;
using AmenityBookingService.Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using Shared.SharedLibrary.Exceptions;

namespace AmenityBookingService.Application.Features.Amenities.Queries;

/// <summary>
/// Query to retrieve a single amenity by its unique identifier.
/// </summary>
public class GetAmenityByIdQuery : IRequest<AmenityResponseDto>
{
    /// <summary>
    /// Gets or sets the unique identifier of the amenity to retrieve.
    /// </summary>
    public Guid AmenityId { get; set; }
}

/// <summary>
/// Handler for processing <see cref="GetAmenityByIdQuery"/> to retrieve an amenity by ID.
/// </summary>
/// <remarks>
/// Business Rules:
/// - Amenity must exist in the system
/// - Returns full amenity details including all properties
/// </remarks>
public class GetAmenityByIdHandler : IRequestHandler<GetAmenityByIdQuery, AmenityResponseDto>
{
    private readonly IAmenityRepository _repository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAmenityByIdHandler"/> class.
    /// </summary>
    /// <param name="repository">The amenity repository for data access.</param>
    /// <param name="mapper">The AutoMapper instance for object mapping.</param>
    public GetAmenityByIdHandler(IAmenityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the retrieval of a specific amenity by its unique identifier.
    /// </summary>
    /// <param name="request">The get amenity by ID query containing the amenity ID.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The amenity response DTO containing all amenity details.</returns>
    /// <exception cref="NotFoundException">Thrown when the amenity with the specified ID does not exist.</exception>
    public async Task<AmenityResponseDto> Handle(
        GetAmenityByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var amenity = await _repository.GetByIdAsync(request.AmenityId, cancellationToken);

        if (amenity == null)
            throw new NotFoundException(
                string.Format(ErrorMessages.AmenityNotFound, request.AmenityId)
            );

        return _mapper.Map<AmenityResponseDto>(amenity);
    }
}
