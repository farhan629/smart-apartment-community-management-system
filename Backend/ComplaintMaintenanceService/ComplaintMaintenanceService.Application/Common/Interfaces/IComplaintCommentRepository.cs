using ComplaintMaintenanceService.Domain.Entities;

namespace ComplaintMaintenanceService.Application.Interfaces.Repositories;

public interface IComplaintCommentRepository
{
    Task<List<ComplaintComment>> GetByComplaintIdAsync(
        Guid complaintId,
        CancellationToken ct = default
    );
    Task<List<ComplaintComment>> GetByStaffIdAsync(Guid staffId, CancellationToken ct = default);
    Task<ComplaintComment?> GetRatingByComplaintIdAsync(
        Guid complaintId,
        Guid commentedBy,
        CancellationToken ct = default
    );
    Task<ComplaintComment> AddAsync(ComplaintComment comment, CancellationToken ct = default);
}
