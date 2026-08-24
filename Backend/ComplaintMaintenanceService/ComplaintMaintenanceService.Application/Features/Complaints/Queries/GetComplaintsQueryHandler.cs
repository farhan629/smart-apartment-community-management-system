using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Common.Pagination;
using ComplaintMaintenanceService.Application.Features.Complaints.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;
using Shared.SharedLibrary.Constants;

namespace ComplaintMaintenanceService.Application.Features.Complaints.Queries;

public class GetComplaintsQuery : IRequest<PagedResult<ComplaintSummaryDto>>
{
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public Guid? CategoryId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = PaginationConstants.DefaultPageNumber;
    public int Limit { get; set; } = PaginationConstants.DefaultPageSize;
    public bool IsResident { get; set; }
    public Guid? CurrentUserId { get; set; }
    public Guid CurrentRoleId { get; set; }
}

public class GetComplaintsQueryHandler
    : IRequestHandler<GetComplaintsQuery, PagedResult<ComplaintSummaryDto>>
{
    private readonly IComplaintRepository _complaintRepo;
    private readonly IRefTermRepository _refTermRepo;
    private readonly IStaffRepository _staffRepo;

    public GetComplaintsQueryHandler(
        IComplaintRepository complaintRepo,
        IRefTermRepository refTermRepo,
        IStaffRepository staffRepo
    )
    {
        _complaintRepo = complaintRepo;
        _refTermRepo = refTermRepo;
        _staffRepo = staffRepo;
    }

    public async Task<PagedResult<ComplaintSummaryDto>> Handle(
        GetComplaintsQuery query,
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

        Guid? residentIdFilter = null;
        Guid? assignedStaffIdFilter = null;
        Guid? deniedAssignmentStatusId = null;

        if (query.CurrentRoleId == RoleIds.Resident || query.CurrentRoleId == RoleIds.Tenant)
        {
            residentIdFilter = query.CurrentUserId;
        }
        else if (query.CurrentRoleId == RoleIds.Staff)
        {
            var staff =
                query.CurrentUserId.HasValue
                    ? await _staffRepo.GetByUserIdAsync(query.CurrentUserId.Value, ct)
                    : null;

            if (staff is null)
            {
                return new PagedResult<ComplaintSummaryDto>
                {
                    Items = new List<ComplaintSummaryDto>(),
                    TotalCount = 0,
                    Page = pageNumber,
                    Limit = pageSize,
                };
            }

            assignedStaffIdFilter = staff.Id;
            deniedAssignmentStatusId = (
                await _refTermRepo.GetByCodeAsync(ComplaintConstants.AssignmentStatusCodes.Denied)
            )?.Id;
        }

        Guid? statusId = null;
        Guid? priorityId = null;

        if (!string.IsNullOrEmpty(query.Status))
            statusId = (await _refTermRepo.GetByCodeAsync(query.Status))?.Id;

        if (!string.IsNullOrEmpty(query.Priority))
            priorityId = (await _refTermRepo.GetByCodeAsync(query.Priority))?.Id;

        (List<ComplaintMaintenanceService.Domain.Entities.Complaint> items, int totalCount) =
            await _complaintRepo.GetPagedAsync(
                residentIdFilter,
                assignedStaffIdFilter,
                deniedAssignmentStatusId,
                statusId,
                priorityId,
                query.CategoryId,
                query.FromDate,
                query.ToDate,
                pageNumber,
                pageSize,
                ct
            );

        var dtos = items.Select(c => new ComplaintSummaryDto
        {
            ComplaintId = c.Id,
            Description = c.Description,
            Status = c.Status?.DisplayName ?? string.Empty,
            Priority = c.Priority?.DisplayName ?? string.Empty,
            Category = c.Category?.Name ?? string.Empty,
            ScheduledDate = c.ScheduledDate?.ToString(ComplaintConstants.DateFormats.OutputDate),
            CreatedAt = c.CreatedAt,
        });

        return new PagedResult<ComplaintSummaryDto>
        {
            Items = dtos.ToList(),
            TotalCount = totalCount,
            Page = pageNumber,
            Limit = pageSize,
        };
    }
}