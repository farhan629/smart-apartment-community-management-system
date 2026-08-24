using AmenityBookingService.Application.Features.Slots.DTO;
using AmenityBookingService.Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.DTO.Common;
using Shared.SharedLibrary.Exceptions;

namespace AmenityBookingService.Application.Features.Slots.Queries;

/// <summary>
/// Query to retrieve available slots for an amenity with amenity context and spot availability.
/// </summary>
public class GetAvailableSlotsQuery : IRequest<AvailableSlotsResponseDto>
{
    /// <summary>
    /// Gets or sets the unique identifier of the amenity to get available slots for.
    /// </summary>
    public Guid AmenityId { get; set; }

    /// <summary>
    /// Gets or sets the optional date filter for available slots.
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// Gets or sets the page number (default: 1).
    /// </summary>
    public int PageNumber { get; set; } = PaginationConstants.DefaultPageNumber;

    /// <summary>
    /// Gets or sets the page size (default: 10).
    /// </summary>
    public int PageSize { get; set; } = PaginationConstants.DefaultPageSize;
}

/// <summary>
/// Handler for processing <see cref="GetAvailableSlotsQuery"/> to retrieve available slots with computed availability.
/// </summary>
/// <remarks>
/// Business Rules:
/// - Amenity must exist
/// - Pagination parameters are validated and sanitized
/// - Maximum page size is enforced to prevent performance issues
/// - Available spots are computed as (MaxCapacity - CurrentBookings)
/// - Response includes amenity metadata for context
/// - Slots with zero available spots are still returned (fully booked)
/// - If no date is provided, returns slots for the current date
/// </remarks>
public class GetAvailableSlotsHandler
    : IRequestHandler<GetAvailableSlotsQuery, AvailableSlotsResponseDto>
{
    private readonly ISlotRepository _slotRepository;
    private readonly IAmenityRepository _amenityRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAvailableSlotsHandler"/> class.
    /// </summary>
    /// <param name="slotRepository">The slot repository for slot data access.</param>
    /// <param name="amenityRepository">The amenity repository for amenity data access and validation.</param>
    /// <param name="mapper">The AutoMapper instance for object mapping.</param>
    public GetAvailableSlotsHandler(
        ISlotRepository slotRepository,
        IAmenityRepository amenityRepository,
        IMapper mapper
    )
    {
        _slotRepository = slotRepository;
        _amenityRepository = amenityRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the retrieval of available slots with computed available spots and amenity metadata.
    /// </summary>
    /// <param name="request">The get available slots query containing amenity ID, date filter, and pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A paginated list of available slot response DTOs with amenity metadata and pagination info.</returns>
    /// <exception cref="NotFoundException">Thrown when the amenity does not exist.</exception>
    public async Task<AvailableSlotsResponseDto> Handle(
        GetAvailableSlotsQuery request,
        CancellationToken cancellationToken
    )
    {
        var amenity = await _amenityRepository.GetAmenityWithSlotTypeAsync(
            request.AmenityId,
            cancellationToken
        );
        if (amenity == null)
            throw new NotFoundException(
                string.Format(
                    AmenityBookingService.Application.Constants.ErrorMessages.AmenityNotFound,
                    request.AmenityId
                )
            );

        var pageNumber =
            request.PageNumber < PaginationConstants.MinPageNumber
                ? PaginationConstants.DefaultPageNumber
                : request.PageNumber;

        var pageSize =
            request.PageSize < PaginationConstants.MinPageSize
                ? PaginationConstants.DefaultPageSize
                : request.PageSize;

        if (pageSize > PaginationConstants.MaxPageSize)
            pageSize = PaginationConstants.MaxPageSize;

        var filterDate = request.Date.HasValue
            ? DateTime.SpecifyKind(request.Date.Value.Date, DateTimeKind.Utc)
            : (DateTime?)null;

        var slots = await _slotRepository.GetAvailableSlotsAsync(
            request.AmenityId,
            filterDate,
            pageNumber,
            pageSize,
            cancellationToken
        );

        var totalCount = await _slotRepository.GetAvailableSlotsCountAsync(
            request.AmenityId,
            filterDate,
            cancellationToken
        );

        var slotDtos = _mapper.Map<List<AvailableSlotResponseDto>>(slots);

        foreach (var slotDto in slotDtos)
        {
            var currentBookings = await _slotRepository.GetCurrentBookingsCountForSlotAsync(
                slotDto.SlotId,
                cancellationToken
            );

            slotDto.CurrentBookings = currentBookings;
            slotDto.AvailableSpots = slotDto.MaxCapacity - currentBookings;
        }

        var totalPages = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;

        var response = _mapper.Map<AvailableSlotsResponseDto>(amenity);
        response.Slots = slotDtos;
        response.Pagination = new PaginationDto
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasPreviousPage = pageNumber > PaginationConstants.MinPageNumber,
            HasNextPage = pageNumber < totalPages,
        };

        return response;
    }
}
