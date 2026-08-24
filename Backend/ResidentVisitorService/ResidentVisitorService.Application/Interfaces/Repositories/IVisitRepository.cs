using ResidentVisitorService.Domain.Entities;

namespace ResidentVisitorService.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Visit data access operations.
/// </summary>
public interface IVisitRepository
{
    /// <summary>Gets a visit by its unique identifier, including all navigation properties.</summary>
    Task<Visit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Returns a paginated, filtered list of active visits.</summary>
    Task<(int TotalCount, List<Visit> Items)> GetAllAsync(
        Guid? visitorId,
        Guid? hostUserId,
        Guid? flatId,
        string? status,
        DateOnly? startDate,
        DateOnly? endDate,
        string sortBy,
        string sortOrder,
        int page,
        int limit,
        CancellationToken cancellationToken = default
    );

    /// <summary>Adds a new visit to the database.</summary>
    Task<Visit> AddAsync(Visit visit, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing visit.</summary>
    Task UpdateAsync(Visit visit, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes a visit by setting IsActive to false.</summary>
    Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns true if an active (PENDING or APPROVED) visit already exists
    /// for the same visitor and host with overlapping dates.
    /// Used to prevent duplicate visit registration.
    /// </summary>
    Task<bool> HasActiveVisitAsync(
        Guid visitorId,
        Guid hostUserId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default
    );
}
