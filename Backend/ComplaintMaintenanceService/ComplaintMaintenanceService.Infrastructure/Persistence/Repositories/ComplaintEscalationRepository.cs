using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Domain.Entities;
using ComplaintMaintenanceService.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace ComplaintMaintenanceService.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IComplaintEscalationRepository"/>.
/// </summary>
public class ComplaintEscalationRepository : IComplaintEscalationRepository
{
    private readonly AppDbContext _db;

    public ComplaintEscalationRepository(AppDbContext db) => _db = db;

    public async Task<ComplaintEscalation?> GetByComplaintIdAsync(
        Guid complaintId,
        CancellationToken ct = default
    ) =>
        await _db.ComplaintEscalations.FirstOrDefaultAsync(
            e => e.ComplaintId == complaintId && e.IsActive,
            ct
        );

    public async Task<List<ComplaintEscalation>> GetUnresolvedAsync(
        CancellationToken ct = default
    ) =>
        await _db
            .ComplaintEscalations.Include(e => e.Complaint)
            .Where(e => !e.ResolvedAfterEscalation && e.IsActive)
            .ToListAsync(ct);

    public async Task<ComplaintEscalation> AddAsync(
        ComplaintEscalation escalation,
        CancellationToken ct = default
    )
    {
        _db.ComplaintEscalations.Add(escalation);
        await _db.SaveChangesAsync(ct);
        return escalation;
    }

    public async Task UpdateAsync(ComplaintEscalation escalation, CancellationToken ct = default)
    {
        _db.ComplaintEscalations.Update(escalation);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken ct = default) =>
        await _db.ComplaintEscalations.Where(e => e.IsActive).CountAsync(ct);
}
