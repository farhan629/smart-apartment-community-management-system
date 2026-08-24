using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Domain.Entities;
using ComplaintMaintenanceService.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace ComplaintMaintenanceService.Infrastructure.Persistence.Repositories;

public class ComplaintCommentRepository : IComplaintCommentRepository
{
    private readonly AppDbContext _db;

    public ComplaintCommentRepository(AppDbContext db) => _db = db;

    public async Task<List<ComplaintComment>> GetByComplaintIdAsync(
        Guid complaintId,
        CancellationToken ct = default
    ) =>
        await _db
            .ComplaintComments.Where(c => c.ComplaintId == complaintId && c.IsActive)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

    public async Task<List<ComplaintComment>> GetByStaffIdAsync(
        Guid staffId,
        CancellationToken ct = default
    ) =>
        await _db
            .ComplaintComments.Where(c =>
                c.IsActive
                && _db.ComplaintAssignments.Any(a =>
                    a.ComplaintId == c.ComplaintId && a.StaffId == staffId && a.IsActive
                )
            )
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

    public async Task<ComplaintComment?> GetRatingByComplaintIdAsync(
        Guid complaintId,
        Guid commentedBy,
        CancellationToken ct = default
    ) =>
        await _db.ComplaintComments.FirstOrDefaultAsync(
            c =>
                c.ComplaintId == complaintId
                && c.CommentedBy == commentedBy
                && c.StaffRating != null,
            ct
        );

    public async Task<ComplaintComment> AddAsync(
        ComplaintComment comment,
        CancellationToken ct = default
    )
    {
        _db.ComplaintComments.Add(comment);
        await _db.SaveChangesAsync(ct);
        return comment;
    }
}
