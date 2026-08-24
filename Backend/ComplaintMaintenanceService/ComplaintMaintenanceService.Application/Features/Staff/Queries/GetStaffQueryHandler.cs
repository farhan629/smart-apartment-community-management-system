using AutoMapper;
using ComplaintMaintenanceService.Application.Common.Pagination;
using ComplaintMaintenanceService.Application.Features.Staff.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.Constants;

namespace ComplaintMaintenanceService.Application.Features.Staff.Queries;

public class GetStaffQuery : IRequest<PagedResult<StaffSummaryDto>>
{
    public int Page { get; set; } = PaginationConstants.DefaultPageNumber;
    public int Limit { get; set; } = PaginationConstants.DefaultPageSize;
}

public class GetStaffQueryHandler : IRequestHandler<GetStaffQuery, PagedResult<StaffSummaryDto>>
{
    private readonly IStaffRepository _staffRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<GetStaffQueryHandler> _logger;

    public GetStaffQueryHandler(
        IStaffRepository staffRepo,
        IMapper mapper,
        ILogger<GetStaffQueryHandler> logger
    )
    {
        _staffRepo = staffRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<StaffSummaryDto>> Handle(
        GetStaffQuery query,
        CancellationToken ct
    )
    {
        _logger.LogInformation("GetStaffQuery - fetching all active staff");

        var pageNumber =
            query.Page < PaginationConstants.MinPageNumber
                ? PaginationConstants.DefaultPageNumber
                : query.Page;

        var pageSize =
            query.Limit < PaginationConstants.MinPageSize
                ? PaginationConstants.DefaultPageSize
                : query.Limit;

        if (pageSize > PaginationConstants.MaxPageSize)
        {
            pageSize = PaginationConstants.MaxPageSize;
        }

        var all = await _staffRepo.GetAllAsync(ct);
        var totalCount = all.Count;
        var paged = all.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResult<StaffSummaryDto>
        {
            Items = _mapper.Map<List<StaffSummaryDto>>(paged),
            TotalCount = totalCount,
            Page = pageNumber,
            Limit = pageSize,
        };
    }
}
