using ComplaintMaintenanceService.Application.Features.Reports.DTOs;
using ComplaintMaintenanceService.Domain.Entities;

namespace ComplaintMaintenanceService.Application.Interfaces.Repositories;

public interface IComplaintRepository
{
    Task<Complaint> AddAsync(Complaint complaint, CancellationToken ct = default);

    Task<Complaint?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task UpdateAsync(Complaint complaint, CancellationToken ct = default);

    Task<(List<Complaint> Items, int TotalCount)> GetPagedAsync(
        Guid? residentId,
        Guid? assignedStaffId,
        Guid? deniedAssignmentStatusId,
        Guid? statusId,
        Guid? priorityId,
        Guid? categoryId,
        DateTime? fromDate,
        DateTime? toDate,
        int page,
        int limit,
        CancellationToken ct = default
    );

    Task<ReportResponseDto> GetReportDataAsync(
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken ct = default
    );
}
