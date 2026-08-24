using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Domain.Entities;
using ComplaintMaintenanceService.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace ComplaintMaintenanceService.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IStaffRepository"/>.
/// Handles persistence of Staff profiles that are created via gRPC
/// during IdentityService staff registration and updated via REST PATCH.
/// </summary>
public class StaffRepository : IStaffRepository
{
    private readonly AppDbContext _db;

    public StaffRepository(AppDbContext db) => _db = db;

    /// <inheritdoc/>
    public async Task<Staff> AddAsync(Staff staff, CancellationToken ct = default)
    {
        _db.Staff.Add(staff);
        await _db.SaveChangesAsync(ct);
        return staff;
    }

    /// <inheritdoc/>
    public async Task<Staff?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Staff.Include(s => s.Category).FirstOrDefaultAsync(s => s.Id == id, ct);

    /// <inheritdoc/>
    public async Task<Staff?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await _db.Staff.Include(s => s.Category).FirstOrDefaultAsync(s => s.UserId == userId, ct);

    /// <inheritdoc/>
    public async Task<List<Staff>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Staff.Include(s => s.Category).Where(s => s.IsActive).ToListAsync(ct);

    /// <inheritdoc/>
    public async Task UpdateAsync(Staff staff, CancellationToken ct = default)
    {
        _db.Staff.Update(staff);
        await _db.SaveChangesAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<List<Staff>> GetByCategoryIdAsync(
        Guid categoryId,
        CancellationToken ct = default
    ) =>
        await _db
            .Staff.Include(s => s.Category)
            .Where(s => s.CategoryId == categoryId && s.IsActive)
            .ToListAsync(ct);
}
