using AmenityBookingService.Domain.Entities;

namespace AmenityBookingService.Application.Interfaces.Repositories;

/// <summary>
/// Repository for reference term operations.
/// </summary>
public interface IRefTermRepository
{
    /// <summary>Gets a reference term by its code.</summary>
    Task<RefTerm?> GetRefTermByCodeAsync(
        string code,
        CancellationToken cancellationToken = default
    );

    /// <summary>Gets a reference term by its unique identifier.</summary>
    Task<RefTerm?> GetRefTermByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
