using ResidentVisitorService.Domain.Entities;

namespace ResidentVisitorService.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for VisitQrToken data access operations.
/// </summary>
public interface IVisitQrTokenRepository
{
    /// <summary>Gets a QR token record by its unique token string.</summary>
    Task<VisitQrToken?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken = default
    );

    /// <summary>Gets a QR token record by the associated visit ID.</summary>
    Task<VisitQrToken?> GetByVisitIdAsync(
        Guid visitId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Adds a new QR token to the database.</summary>
    Task<VisitQrToken> AddAsync(
        VisitQrToken qrToken,
        CancellationToken cancellationToken = default
    );
}
