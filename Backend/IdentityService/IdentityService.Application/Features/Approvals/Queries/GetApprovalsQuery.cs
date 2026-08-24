using AutoMapper;
using IdentityService.Application.Features.Approvals.DTOs;
using IdentityService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.Exceptions;

namespace IdentityService.Application.Features.Approvals.Queries
{
    /// <summary>
    /// Fetches all approvals (optionally filtered by status/userId) or a single approval by id.
    /// Mirrors GET /api/approval?id=&amp;userId=&amp;status=
    /// </summary>
    public class GetApprovalsQuery : IRequest<object>
    {
        /// <summary>Gets or sets the optional approval record identifier to fetch a single record.</summary>
        public Guid? Id { get; set; }

        /// <summary>Gets or sets the optional user identifier filter.</summary>
        public Guid? UserId { get; set; }

        /// <summary>Gets or sets the optional status filter ("pending" or "approved").</summary>
        public string? Status { get; set; }

        /// <summary>Gets or sets the page number for pagination.</summary>
        public int Page { get; set; } = 1;

        /// <summary>Gets or sets the page limit size for pagination.</summary>
        public int Limit { get; set; } = 10;
    }

    /// <summary>
    /// Handler for processing the <see cref="GetApprovalsQuery"/>.
    /// </summary>
    public class GetApprovalsQueryHandler : IRequestHandler<GetApprovalsQuery, object>
    {
        private readonly IFlatOccupancyRepository _flatOccupancyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetApprovalsQueryHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetApprovalsQueryHandler"/> class.
        /// </summary>
        /// <param name="flatOccupancyRepository">The repository for flat occupancy requests.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public GetApprovalsQueryHandler(
            IFlatOccupancyRepository flatOccupancyRepository,
            IMapper mapper,
            ILogger<GetApprovalsQueryHandler> logger
        )
        {
            _flatOccupancyRepository = flatOccupancyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the execution of the query to get approvals.
        /// </summary>
        /// <param name="request">The query containing parameters.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A single approval detail DTO or a paginated list of approvals.</returns>
        public async Task<object> Handle(
            GetApprovalsQuery request,
            CancellationToken cancellationToken
        )
        {
            if (request.Id.HasValue)
            {
                var occupancy =
                    await _flatOccupancyRepository.GetByIdAsync(request.Id.Value)
                    ?? throw new NotFoundException(ExceptionMessages.NotFound);

                return _mapper.Map<ApprovalDetailDto>(occupancy);
            }

            var pageNumber =
                request.Page < PaginationConstants.MinPageNumber
                    ? PaginationConstants.DefaultPageNumber
                    : request.Page;

            var pageSize =
                request.Limit < PaginationConstants.MinPageSize
                    ? PaginationConstants.DefaultPageSize
                    : request.Limit;

            if (pageSize > PaginationConstants.MaxPageSize)
            {
                pageSize = PaginationConstants.MaxPageSize;
            }

            var (total, items) = await _flatOccupancyRepository.GetAllAsync(
                pageNumber,
                pageSize,
                request.Status,
                request.UserId
            );

            _logger.LogInformation(
                "Fetched approvals page {Page} (status={Status}, userId={UserId})",
                pageNumber,
                request.Status,
                request.UserId
            );

            return new PaginatedApprovalResponseDto
            {
                Page = pageNumber,
                Limit = pageSize,
                Total = total,
                Items = _mapper.Map<IEnumerable<ApprovalDetailDto>>(items),
            };
        }
    }
}
