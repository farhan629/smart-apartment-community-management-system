using AmenityBookingService.Application.Features.Amenities.DTO;
using AmenityBookingService.Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.DTO.Common;

namespace AmenityBookingService.Application.Features.Amenities.Queries;

/// <summary>
/// Query to retrieve a paginated list of amenities with optional search and slot type filter.
/// </summary>
public class GetAmenitiesQuery : IRequest<AmenityListResponseDto>
{
    /// <summary>
    /// Gets or sets the page number (default: 1).
    /// </summary>
    public int PageNumber { get; set; } = PaginationConstants.DefaultPageNumber;

    /// <summary>
    /// Gets or sets the page size (default: 10).
    /// </summary>
    public int PageSize { get; set; } = PaginationConstants.DefaultPageSize;

    /// <summary>
    /// Gets or sets the optional name search filter.
    /// </summary>
    public string? SearchName { get; set; }

    /// <summary>
    /// Gets or sets the optional slot type code filter.
    /// </summary>
    public string? SlotType { get; set; }
}

/// <summary>
/// Handler for processing <see cref="GetAmenitiesQuery"/> to retrieve a paginated list of amenities.
/// </summary>
/// <remarks>
/// Business Rules:
/// - Pagination parameters are validated and sanitized
/// - Maximum page size is enforced to prevent performance issues
/// - Search by name uses partial matching
/// - Filter by slot type uses exact code matching
/// </remarks>
public class GetAmenitiesHandler : IRequestHandler<GetAmenitiesQuery, AmenityListResponseDto>
{
    private readonly IAmenityRepository _repository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAmenitiesHandler"/> class.
    /// </summary>
    /// <param name="repository">The amenity repository for data access.</param>
    /// <param name="mapper">The AutoMapper instance for object mapping.</param>
    public GetAmenitiesHandler(IAmenityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the retrieval of a paginated list of amenities with optional filters.
    /// </summary>
    /// <param name="request">The get amenities query containing pagination and filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A paginated list of amenity response DTOs with pagination metadata.</returns>
    public async Task<AmenityListResponseDto> Handle(
        GetAmenitiesQuery request,
        CancellationToken cancellationToken
    )
    {
        // Validate and sanitize pagination parameters
        var pageNumber =
            request.PageNumber < PaginationConstants.MinPageNumber
                ? PaginationConstants.DefaultPageNumber
                : request.PageNumber;

        var pageSize =
            request.PageSize < PaginationConstants.MinPageSize
                ? PaginationConstants.DefaultPageSize
                : request.PageSize;

        // Enforce maximum page size to prevent performance issues
        if (pageSize > PaginationConstants.MaxPageSize)
        {
            pageSize = PaginationConstants.MaxPageSize;
        }

        // Fetch amenities with pagination and filters
        var amenities = await _repository.GetAllAsync(
            pageNumber,
            pageSize,
            request.SearchName,
            request.SlotType,
            cancellationToken
        );

        // Get total count for pagination metadata
        var totalCount = await _repository.GetTotalCountAsync(
            request.SearchName,
            request.SlotType,
            cancellationToken
        );

        // Map entities to response DTOs
        var data = _mapper.Map<List<AmenityResponseDto>>(amenities);

        // Calculate total pages (handle division by zero)
        var totalPages = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;

        // Build and return response with pagination metadata
        return new AmenityListResponseDto
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
