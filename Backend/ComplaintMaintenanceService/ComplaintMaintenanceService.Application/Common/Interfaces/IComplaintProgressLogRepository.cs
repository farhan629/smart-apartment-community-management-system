using ComplaintMaintenanceService.Domain.Entities;

namespace ComplaintMaintenanceService.Application.Interfaces.Repositories;

public interface IComplaintProgressLogRepository
{
    Task<ComplaintProgressLog> AddAsync(ComplaintProgressLog log, CancellationToken ct = default);
    Task<List<ComplaintProgressLog>> GetByComplaintIdAsync(
        Guid complaintId,
        CancellationToken ct = default
    );
}
