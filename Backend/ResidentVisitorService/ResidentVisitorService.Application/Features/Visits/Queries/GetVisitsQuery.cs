using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visits.DTOs;
using ResidentVisitorService.Application.Interfaces.Repositories;
using ResidentVisitorService.Domain.Entities;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.DTO.Common;
using Shared.SharedLibrary.Services;

namespace ResidentVisitorService.Application.Features.Visits.Queries;

/// <summary>Query to retrieve a paginated list of visits with optional filters.</summary>
public class GetVisitsQuery : IRequest<GetVisitsResponse>
{
    public Guid? Id { get; set; }
    public Guid? VisitorId { get; set; }
    public Guid? HostUserId { get; set; }
    public Guid? FlatId { get; set; }
    public string? Status { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string SortBy { get; set; } = ResidentVisitorConstants.Pagination.DefaultSortBy;
    public string SortOrder { get; set; } = ResidentVisitorConstants.Pagination.DefaultSortOrder;
    public int Page { get; set; } = PaginationConstants.DefaultPageNumber;
    public int Limit { get; set; } = PaginationConstants.DefaultPageSize;
}

/// <summary>Handles the <see cref="GetVisitsQuery"/> request.</summary>
public class GetVisitsQueryHandler : IRequestHandler<GetVisitsQuery, GetVisitsResponse>
{
    private readonly IVisitRepository _visitRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly ILogger<GetVisitsQueryHandler> _logger;

    public GetVisitsQueryHandler(
        IVisitRepository visitRepository,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        ILogger<GetVisitsQueryHandler> logger
    )
    {
        _visitRepository = visitRepository;
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetVisitsResponse> Handle(
        GetVisitsQuery request,
        CancellationToken cancellationToken
    )
    {
        var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

        var effectiveHostUserId = request.HostUserId;
        if (userRole == ResidentVisitorConstants.Roles.Resident)
        {
            effectiveHostUserId = _currentUserService.UserId;
        }

        if (request.Id.HasValue)
        {
            _logger.LogInformation("Fetching visit with id {VisitId}", request.Id);
            var visit =
                await _visitRepository.GetByIdAsync(request.Id.Value, cancellationToken)
                ?? throw new KeyNotFoundException(
                    string.Format(ResidentVisitorConstants.Errors.VisitNotFound, request.Id)
                );

            if (
                userRole == ResidentVisitorConstants.Roles.Resident
                && visit.HostUserId != _currentUserService.UserId
            )
                throw new KeyNotFoundException(
                    string.Format(ResidentVisitorConstants.Errors.VisitNotFound, request.Id)
                );

            return new GetVisitsResponse
            {
                Items = [_mapper.Map<VisitResponseDto>(visit)],
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
            "Fetching visits — status: {Status}, page: {Page}, limit: {Limit}",
            request.Status,
            request.Page,
            request.Limit
        );

        var (totalCount, items) = await _visitRepository.GetAllAsync(
            request.VisitorId,
            effectiveHostUserId,
            request.FlatId,
            request.Status,
            request.StartDate,
            request.EndDate,
            request.SortBy,
            request.SortOrder,
            request.Page,
            request.Limit,
            cancellationToken
        );

        var totalPages = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / request.Limit) : 0;

        return new GetVisitsResponse
        {
            Items = _mapper.Map<List<VisitResponseDto>>(items),
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
