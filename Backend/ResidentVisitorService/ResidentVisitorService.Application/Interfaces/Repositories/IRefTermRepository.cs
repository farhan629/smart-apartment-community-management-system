using ResidentVisitorService.Domain.Entities;

namespace ResidentVisitorService.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for RefTerm (lookup/reference data) access.
/// </summary>
public interface IRefTermRepository
{
    /// <summary>Gets all active RefTerms belonging to a given RefSet code.</summary>
    Task<List<RefTerm>> GetByRefSetCodeAsync(
        string refSetCode,
        CancellationToken cancellationToken = default
    );

    /// <summary>Gets a single RefTerm by its unique identifier.</summary>
    Task<RefTerm?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets a single RefTerm by its code within a RefSet.</summary>
    Task<RefTerm?> GetByCodeAsync(
        string refSetCode,
        string termCode,
        CancellationToken cancellationToken = default
    );
}
