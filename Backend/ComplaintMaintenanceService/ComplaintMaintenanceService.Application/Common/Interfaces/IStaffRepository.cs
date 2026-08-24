using ComplaintMaintenanceService.Domain.Entities;

namespace ComplaintMaintenanceService.Application.Interfaces.Repositories;

/// <summary>
/// Defines data access operations for the Staff entity.
/// Used by gRPC service (creation) and REST handlers (read, update).
/// </summary>
public interface IStaffRepository
{
    /// <summary>Persists a new Staff record created via gRPC from IdentityService.</summary>
    Task<Staff> AddAsync(Staff staff, CancellationToken ct = default);

    /// <summary>Retrieves a staff member by their Staff table primary key.</summary>
    Task<Staff?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a staff member by their IdentityService UserId.
    /// Used to check for duplicate staff profiles on re-registration attempts.
    /// </summary>
    Task<Staff?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Returns all active staff members with their category details.</summary>
    Task<List<Staff>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Persists changes to an existing staff profile (category, description, details).</summary>
    Task UpdateAsync(Staff staff, CancellationToken ct = default);

    /// <summary>Returns all active staff members in a given category.</summary>
    Task<List<Staff>> GetByCategoryIdAsync(Guid categoryId, CancellationToken ct = default);
}
