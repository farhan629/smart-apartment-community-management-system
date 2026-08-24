using ComplaintMaintenanceService.Application.Features.Assignments.DTOs;
using ComplaintMaintenanceService.Domain.Entities;

namespace ComplaintMaintenanceService.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for complaint assignment operations.
/// </summary>
public interface IComplaintAssignmentRepository
{
    Task<ComplaintAssignment?> GetByIdAsync(Guid assignmentId, CancellationToken ct = default);
    Task<ComplaintAssignment?> GetActiveByComplaintIdAsync(
        Guid complaintId,
        CancellationToken ct = default
    );
    Task<List<ComplaintAssignment>> GetByComplaintIdAsync(
        Guid complaintId,
        CancellationToken ct = default
    );
    Task<(List<AssignmentResponseDto> Items, int TotalCount)> GetByStaffIdAsync(
        Guid staffId,
        int page,
        int limit,
        CancellationToken ct = default
    );
    Task<ComplaintAssignment> AddAsync(
        ComplaintAssignment assignment,
        CancellationToken ct = default
    );
    Task UpdateAsync(ComplaintAssignment assignment, CancellationToken ct = default);
}