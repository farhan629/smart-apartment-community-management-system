using IdentityService.Application.Features.Flats.DTOs;
using IdentityService.Application.Interfaces.Repositories;
using MediatR;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.DTO.Common;

namespace IdentityService.Application.Features.Flats.Queries
{
    /// <summary>
    /// Query used to retrieve a paginated list of flats.
    /// </summary>
    public class GetFlatsQuery : IRequest<PagedFlatResponseDto>
    {
        /// <summary>
        /// Gets or sets the page number to retrieve.
        /// </summary>
        public int PageNumber { get; set; } = PaginationConstants.DefaultPageNumber;

        /// <summary>
        /// Gets or sets the number of records to return per page.
        /// </summary>
        public int PageSize { get; set; } = PaginationConstants.DefaultPageSize;
    }

    /// <summary>
    /// Handles the retrieval of paginated flat data.
    /// </summary>
    public class GetFlatsQueryHandler : IRequestHandler<GetFlatsQuery, PagedFlatResponseDto>
    {
        private readonly IFlatRepository _flatRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFlatsQueryHandler"/> class.
        /// </summary>
        /// <param name="flatRepository">
        /// Repository used to access flat data.
        /// </param>
        public GetFlatsQueryHandler(IFlatRepository flatRepository)
        {
            _flatRepository = flatRepository;
        }

        /// <summary>
        /// Retrieves a paginated list of flats sorted by availability.
        /// Validates pagination parameters, maps entities to response DTOs,
        /// and generates pagination metadata.
        /// </summary>
        /// <param name="request">
        /// The query containing pagination settings.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel the operation.
        /// </param>
        /// <returns>
        /// A <see cref="PagedFlatResponseDto"/> containing the requested flats
        /// and pagination information.
        /// </returns>
        public async Task<PagedFlatResponseDto> Handle(
            GetFlatsQuery request,
            CancellationToken cancellationToken
        )
        {
            var pageNumber =
                request.PageNumber < PaginationConstants.MinPageNumber
                    ? PaginationConstants.DefaultPageNumber
                    : request.PageNumber;

            var pageSize =
                request.PageSize < PaginationConstants.MinPageSize
                    ? PaginationConstants.DefaultPageSize
                : request.PageSize > PaginationConstants.MaxPageSize
                    ? PaginationConstants.MaxPageSize
                : request.PageSize;

            var (totalCount, flats) = await _flatRepository.GetPagedSortedByAvailabilityAsync(
                pageNumber,
                pageSize,
                cancellationToken
            );

            var items = flats
                .Select(f =>
                {
                    bool isAvailable = !(f.FlatOccupancies?.Any(o => o.FlatId == f.Id) ?? false);

                    return new FlatResponseDto
                    {
                        Id = f.Id,
                        Number = f.Number,
                        Block = f.Block,
                        Floor = f.Floor,
                        IsAvailable = isAvailable,
                        CreatedAt = f.CreatedAt,
                    };
                })
                .ToList();

            var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PagedFlatResponseDto
            {
                Items = items,
                Pagination = new PaginationDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    HasPreviousPage = pageNumber > 1,
                    HasNextPage = pageNumber < totalPages,
                },
            };
        }
    }
}
