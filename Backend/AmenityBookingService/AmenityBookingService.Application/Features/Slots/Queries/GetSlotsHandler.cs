using AmenityBookingService.Application.Features.Slots.DTO;
using AmenityBookingService.Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.DTO.Common;
using Shared.SharedLibrary.Exceptions;

namespace AmenityBookingService.Application.Features.Slots.Queries;

/// <summary>
/// Query to retrieve a paginated list of slots for a given amenity.
/// </summary>
public class GetSlotsQuery : IRequest<SlotListResponseDto>
{
    /// <summary>
    /// Gets or sets the unique identifier of the amenity to get slots for.
    /// </summary>
    public Guid AmenityId { get; set; }

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
/// Handler for processing <see cref="GetSlotsQuery"/> to retrieve a paginated list of slots with current booking counts.
/// </summary>
/// <remarks>
/// Business Rules:
/// - Amenity must exist
/// - Pagination parameters are validated and sanitized
/// - Maximum page size is enforced to prevent performance issues
/// - Current booking count is included for each slot
/// - Returns all slots (both available and fully booked)
/// - Slots are ordered by date and time
/// </remarks>
public class GetSlotsHandler : IRequestHandler<GetSlotsQuery, SlotListResponseDto>
{
    private readonly ISlotRepository _slotRepository;
    private readonly IAmenityRepository _amenityRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSlotsHandler"/> class.
    /// </summary>
    /// <param name="slotRepository">The slot repository for slot data access.</param>
    /// <param name="amenityRepository">The amenity repository for amenity validation.</param>
    /// <param name="mapper">The AutoMapper instance for object mapping.</param>
    public GetSlotsHandler(
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
    /// Handles the retrieval of a paginated list of slots with current booking counts.
    /// </summary>
    /// <param name="request">The get slots query containing amenity ID and pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A paginated list of slot response DTOs with current booking counts and pagination metadata.</returns>
    /// <exception cref="NotFoundException">Thrown when the amenity does not exist.</exception>
    public async Task<SlotListResponseDto> Handle(
        GetSlotsQuery request,
        CancellationToken cancellationToken
    )
    {
        var amenity = await _amenityRepository.GetByIdAsync(request.AmenityId, cancellationToken);
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

        var slots = await _slotRepository.GetSlotsByAmenityIdAsync(
            request.AmenityId,
            pageNumber,
            pageSize,
            cancellationToken
        );

        var totalCount = await _slotRepository.GetSlotsCountByAmenityIdAsync(
            request.AmenityId,
            cancellationToken
        );

        var data = _mapper.Map<List<SlotResponseDto>>(slots);

        foreach (var slot in data)
        {
            slot.CurrentBookings = await _slotRepository.GetCurrentBookingsCountForSlotAsync(
                slot.Id,
                cancellationToken
            );
        }

        var totalPages = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;

        return new SlotListResponseDto
        {
            Data = data,
            Pagination = new PaginationDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > PaginationConstants.MinPageNumber,
                HasNextPage = pageNumber < totalPages,
            },
        };
    }
}
