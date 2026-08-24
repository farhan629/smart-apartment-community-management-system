using ComplaintMaintenanceService.Application.Features.Assignments.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Domain.Entities;
using ComplaintMaintenanceService.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace ComplaintMaintenanceService.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IComplaintAssignmentRepository"/>.
/// </summary>
public class ComplaintAssignmentRepository : IComplaintAssignmentRepository
{
    private readonly AppDbContext _db;

    public ComplaintAssignmentRepository(AppDbContext db) => _db = db;

    public async Task<ComplaintAssignment?> GetByIdAsync(
        Guid assignmentId,
        CancellationToken ct = default
    ) =>
        await _db
            .ComplaintAssignments.Include(a => a.Staff)
            .Include(a => a.Status)
            .Include(a => a.Complaint)
            .FirstOrDefaultAsync(a => a.Id == assignmentId, ct);

    public async Task<ComplaintAssignment?> GetActiveByComplaintIdAsync(
        Guid complaintId,
        CancellationToken ct = default
    ) =>
        await _db
            .ComplaintAssignments.Include(a => a.Staff)
            .Include(a => a.Status)
            .Where(a => a.ComplaintId == complaintId && a.IsActive)
            .OrderByDescending(a => a.AssignedDate)
            .FirstOrDefaultAsync(ct);

    public async Task<List<ComplaintAssignment>> GetByComplaintIdAsync(
        Guid complaintId,
        CancellationToken ct = default
    ) =>
        await _db
            .ComplaintAssignments.Include(a => a.Staff)
            .Include(a => a.Status)
            .Where(a => a.ComplaintId == complaintId)
            .OrderByDescending(a => a.AssignedDate)
            .ToListAsync(ct);

    public async Task<(List<AssignmentResponseDto> Items, int TotalCount)> GetByStaffIdAsync(
        Guid staffId,
        int page,
        int limit,
        CancellationToken ct = default
    )
    {
        var baseQuery = _db.ComplaintAssignments.Where(a => a.StaffId == staffId);

        var totalCount = await baseQuery.CountAsync(ct);

        var items = await baseQuery
            .OrderByDescending(a => a.AssignedDate)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(a => new AssignmentResponseDto
            {
                AssignmentId = a.Id,
                ComplaintId = a.ComplaintId,
                StaffId = a.StaffId,
                StaffName = a.Staff != null ? a.Staff.Description : string.Empty,
                Status = a.Status != null ? a.Status.DisplayName : string.Empty,
                AssignedDate = a.AssignedDate,
                DueDate = a.DueDate,
                AcceptedDate = a.AcceptedDate,
                DeniedDate = a.DeniedDate,
                DenialReason = a.DenialReason,
                AssignedBy = a.AssignedBy,
            })
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<ComplaintAssignment> AddAsync(
        ComplaintAssignment assignment,
        CancellationToken ct = default
    )
    {
        _db.ComplaintAssignments.Add(assignment);
        await _db.SaveChangesAsync(ct);
        return assignment;
    }

    public async Task UpdateAsync(ComplaintAssignment assignment, CancellationToken ct = default)
    {
        _db.ComplaintAssignments.Update(assignment);
        await _db.SaveChangesAsync(ct);
    }
}