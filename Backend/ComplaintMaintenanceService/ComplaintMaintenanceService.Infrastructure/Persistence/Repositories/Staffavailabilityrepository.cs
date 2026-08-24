using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Domain.Entities;
using ComplaintMaintenanceService.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComplaintMaintenanceService.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IStaffAvailabilityRepository"/>.
/// Supports the cancel-complaint flow (existing) and the full
/// StaffAvailability REST feature group (GET list, GET single, POST bulk, DELETE).
/// </summary>
public class StaffAvailabilityRepository : IStaffAvailabilityRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<StaffAvailabilityRepository> _logger;

    public StaffAvailabilityRepository(
        AppDbContext context,
        ILogger<StaffAvailabilityRepository> logger
    )
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<StaffAvailability?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context
            .StaffAvailabilities.Include(s => s.Staff)
                .ThenInclude(st => st!.Category)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    /// <inheritdoc/>
    public async Task<StaffAvailability?> GetByIdAndStaffAsync(
        Guid slotId,
        Guid staffId,
        CancellationToken ct = default
    ) =>
        await _context
            .StaffAvailabilities.Include(s => s.Staff)
                .ThenInclude(st => st!.Category)
            .FirstOrDefaultAsync(s => s.Id == slotId && s.StaffId == staffId, ct);

    /// <inheritdoc/>
    public async Task<List<StaffAvailability>> GetFilteredAsync(
        Guid? staffId,
        DateTime? date,
        Guid? categoryId,
        bool? isBooked,
        DateTime? fromDate,
        DateTime? toDate,
        TimeSpan? startTime,
        TimeSpan? endTime,
        CancellationToken ct = default
    )
    {
        var query = _context
            .StaffAvailabilities.Include(s => s.Staff)
                .ThenInclude(st => st!.Category)
            .Where(s => s.IsActive)
            .AsQueryable();

        if (staffId.HasValue)
            query = query.Where(s => s.StaffId == staffId.Value);

        if (date.HasValue)
            query = query.Where(s => s.AvailableDate.Date == date.Value.Date);

        if (categoryId.HasValue)
            query = query.Where(s => s.Staff != null && s.Staff.CategoryId == categoryId.Value);

        if (isBooked.HasValue)
            query = query.Where(s => s.IsBooked == isBooked.Value);

        if (fromDate.HasValue)
            query = query.Where(s => s.AvailableDate.Date >= fromDate.Value.Date);

        if (toDate.HasValue)
            query = query.Where(s => s.AvailableDate.Date <= toDate.Value.Date);

        if (startTime.HasValue)
            query = query.Where(s => s.SlotStartTime >= startTime.Value);

        if (endTime.HasValue)
            query = query.Where(s => s.SlotEndTime <= endTime.Value);

        return await query
            .OrderBy(s => s.AvailableDate)
            .ThenBy(s => s.SlotStartTime)
            .ToListAsync(ct);
    }

    /// <inheritdoc/>
    public async Task AddRangeAsync(List<StaffAvailability> slots, CancellationToken ct = default)
    {
        await _context.StaffAvailabilities.AddRangeAsync(slots, ct);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Added {Count} availability slots", slots.Count);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(StaffAvailability slot, CancellationToken ct = default)
    {
        _context.StaffAvailabilities.Update(slot);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Staff availability slot {SlotId} updated", slot.Id);
    }
}
