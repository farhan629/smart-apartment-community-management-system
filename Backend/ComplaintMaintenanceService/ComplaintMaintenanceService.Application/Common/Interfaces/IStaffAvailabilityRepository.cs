using ComplaintMaintenanceService.Domain.Entities;

namespace ComplaintMaintenanceService.Application.Interfaces.Repositories;

/// <summary>
/// Data access contract for StaffAvailability slots.
/// </summary>
public interface IStaffAvailabilityRepository
{
    /// <summary>Gets a single slot by its primary key (used by cancel-complaint flow).</summary>
    Task<StaffAvailability?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Gets a single slot by its ID scoped to a specific staff member.</summary>
    Task<StaffAvailability?> GetByIdAndStaffAsync(
        Guid slotId,
        Guid staffId,
        CancellationToken ct = default
    );

    /// <summary>Returns filtered availability slots across staff members.</summary>
    Task<List<StaffAvailability>> GetFilteredAsync(
        Guid? staffId,
        DateTime? date,
        Guid? categoryId,
        bool? isBooked,
        DateTime? fromDate,
        DateTime? toDate,
        TimeSpan? startTime,
        TimeSpan? endTime,
        CancellationToken ct = default
    );

    /// <summary>Bulk-inserts a list of availability slots.</summary>
    Task AddRangeAsync(List<StaffAvailability> slots, CancellationToken ct = default);

    /// <summary>Persists changes to an existing slot (booking, cancellation).</summary>
    Task UpdateAsync(StaffAvailability slot, CancellationToken ct = default);
}
