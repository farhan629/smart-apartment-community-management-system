using ComplaintMaintenanceService.Application.Common.Pagination;
using ComplaintMaintenanceService.Application.Features.Assignments.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.Constants;

namespace ComplaintMaintenanceService.Application.Features.Assignments.Queries;

public class GetStaffAssignmentHistoryQuery : IRequest<PagedResult<AssignmentResponseDto>>
{
    public Guid CurrentUserId { get; set; }
    public int Page { get; set; } = PaginationConstants.DefaultPageNumber;
    public int Limit { get; set; } = PaginationConstants.DefaultPageSize;
}

public class GetStaffAssignmentHistoryQueryHandler
    : IRequestHandler<GetStaffAssignmentHistoryQuery, PagedResult<AssignmentResponseDto>>
{
    private readonly IComplaintAssignmentRepository _assignmentRepo;
    private readonly IStaffRepository _staffRepo;
    private readonly ILogger<GetStaffAssignmentHistoryQueryHandler> _logger;

    public GetStaffAssignmentHistoryQueryHandler(
        IComplaintAssignmentRepository assignmentRepo,
        IStaffRepository staffRepo,
        ILogger<GetStaffAssignmentHistoryQueryHandler> logger
    )
    {
        _assignmentRepo = assignmentRepo;
        _staffRepo = staffRepo;
        _logger = logger;
    }

    public async Task<PagedResult<AssignmentResponseDto>> Handle(
        GetStaffAssignmentHistoryQuery query,
        CancellationToken ct
    )
    {
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

        var staff = await _staffRepo.GetByUserIdAsync(query.CurrentUserId, ct);

        if (staff is null)
        {
            return new PagedResult<AssignmentResponseDto>
            {
                Items = new List<AssignmentResponseDto>(),
                TotalCount = 0,
                Page = pageNumber,
                Limit = pageSize,
            };
        }

        (List<AssignmentResponseDto> items, int totalCount) =
            await _assignmentRepo.GetByStaffIdAsync(staff.Id, pageNumber, pageSize, ct);

        _logger.LogInformation(
            "Assignment history fetched for staff {StaffId} (user {UserId})",
            staff.Id,
            query.CurrentUserId
        );

        return new PagedResult<AssignmentResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = pageNumber,
            Limit = pageSize,
        };
    }
}