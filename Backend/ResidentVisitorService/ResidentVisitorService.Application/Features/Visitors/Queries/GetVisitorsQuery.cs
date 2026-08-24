using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visitors.DTOs;
using ResidentVisitorService.Application.Interfaces.Repositories;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.DTO.Common;
using Shared.SharedLibrary.Services;

namespace ResidentVisitorService.Application.Features.Visitors.Queries;

/// <summary>Query to retrieve a paginated list of visitors.</summary>
public class GetVisitorsQuery : IRequest<GetVisitorsResponse>
{
    public Guid? Id { get; set; }
    public string? Search { get; set; }
    public string SortBy { get; set; } = ResidentVisitorConstants.Pagination.DefaultSortBy;
    public string SortOrder { get; set; } = ResidentVisitorConstants.Pagination.DefaultSortOrder;
    public int Page { get; set; } = PaginationConstants.DefaultPageNumber;
    public int Limit { get; set; } = PaginationConstants.DefaultPageSize;
}

/// <summary>Handles the <see cref="GetVisitorsQuery"/> request.</summary>
public class GetVisitorsQueryHandler : IRequestHandler<GetVisitorsQuery, GetVisitorsResponse>
{
    private readonly IVisitorRepository _visitorRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly ILogger<GetVisitorsQueryHandler> _logger;

    public GetVisitorsQueryHandler(
        IVisitorRepository visitorRepository,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        ILogger<GetVisitorsQueryHandler> logger
    )
    {
        _visitorRepository = visitorRepository;
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetVisitorsResponse> Handle(
        GetVisitorsQuery request,
        CancellationToken cancellationToken
    )
    {
        var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
        Guid? hostUserId =
            userRole == ResidentVisitorConstants.Roles.Resident ? _currentUserService.UserId : null;

        if (request.Id.HasValue)
        {
            _logger.LogInformation("Fetching visitor with id {VisitorId}", request.Id);
            var visitor =
                await _visitorRepository.GetByIdAsync(request.Id.Value, cancellationToken)
                ?? throw new KeyNotFoundException(
                    string.Format(ResidentVisitorConstants.Errors.VisitorNotFound, request.Id)
                );

            return new GetVisitorsResponse
            {
                Items = [_mapper.Map<VisitorResponseDto>(visitor)],
                Pagination = new PaginationDto
                {
                    PageNumber = PaginationConstants.MinPageNumber,
                    PageSize = PaginationConstants.MinPageSize,
                    TotalCount = PaginationConstants.MinPageSize,
                    TotalPages = PaginationConstants.MinPageNumber,
                    HasPreviousPage = false,
                    HasNextPage = false,
                },
            };
        }

        _logger.LogInformation(
            "Fetching visitors — search: {Search}, page: {Page}, limit: {Limit}",
            request.Search,
            request.Page,
            request.Limit
        );

        var (totalCount, items) = await _visitorRepository.GetAllAsync(
            request.Search,
            request.SortBy,
            request.SortOrder,
            request.Page,
            request.Limit,
            hostUserId,
            cancellationToken
        );

        var totalPages = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / request.Limit) : 0;

        return new GetVisitorsResponse
        {
            Items = _mapper.Map<List<VisitorResponseDto>>(items),
            Pagination = new PaginationDto
            {
                PageNumber = request.Page,
                PageSize = request.Limit,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = request.Page > PaginationConstants.MinPageNumber,
                HasNextPage = request.Page < totalPages,
            },
        };
    }
}
