using ResidentVisitorService.Domain.Entities;

namespace ResidentVisitorService.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Visitor data access operations.
/// </summary>
public interface IVisitorRepository
{
    /// <summary>Gets a visitor by their unique identifier.</summary>
    Task<Visitor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets a visitor by their phone number.</summary>
    Task<Visitor?> GetByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default
    );

    /// <summary>Returns a paginated list of active visitors with optional search.
    /// When <paramref name="hostUserId"/> is provided, only visitors who have visited
    /// that resident (via the visits table) are returned.</summary>
    Task<(int TotalCount, List<Visitor> Items)> GetAllAsync(
        string? search,
        string sortBy,
        string sortOrder,
        int page,
        int limit,
        Guid? hostUserId = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>Adds a new visitor to the database.</summary>
    Task<Visitor> AddAsync(Visitor visitor, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing visitor.</summary>
    Task UpdateAsync(Visitor visitor, CancellationToken cancellationToken = default);

    /// <summary>Updates only the photo URL of an existing visitor.</summary>
    Task UpdatePhotoUrlAsync(
        Guid id,
        string photoUrl,
        CancellationToken cancellationToken = default
    );

    /// <summary>Soft-deletes a visitor by setting IsActive to false.</summary>
    Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Checks whether a phone number already exists for a different visitor.</summary>
    Task<bool> PhoneNumberExistsAsync(
        string phoneNumber,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    );
}
