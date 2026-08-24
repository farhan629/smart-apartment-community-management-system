using ComplaintMaintenanceService.Domain.Entities;

namespace ComplaintMaintenanceService.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for complaint escalation operations.
/// </summary>
public interface IComplaintEscalationRepository
{
    Task<ComplaintEscalation?> GetByComplaintIdAsync(
        Guid complaintId,
        CancellationToken ct = default
    );
    Task<List<ComplaintEscalation>> GetUnresolvedAsync(CancellationToken ct = default);
    Task<ComplaintEscalation> AddAsync(
        ComplaintEscalation escalation,
        CancellationToken ct = default
    );
    Task UpdateAsync(ComplaintEscalation escalation, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(CancellationToken ct = default);
}
