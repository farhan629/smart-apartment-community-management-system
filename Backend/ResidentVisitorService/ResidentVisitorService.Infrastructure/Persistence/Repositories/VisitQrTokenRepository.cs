using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResidentVisitorService.Application.Interfaces.Repositories;
using ResidentVisitorService.Domain.Entities;
using ResidentVisitorService.Infrastructure.Persistence.DBContext;

namespace ResidentVisitorService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for <see cref="VisitQrToken"/> data access.
/// </summary>
public class VisitQrTokenRepository : IVisitQrTokenRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<VisitQrTokenRepository> _logger;

    public VisitQrTokenRepository(AppDbContext context, ILogger<VisitQrTokenRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<VisitQrToken?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .VisitQrTokens.Include(q => q.Visit)
                .ThenInclude(v => v!.Visitor)
                    .ThenInclude(vis => vis!.VisitorType)
            .Include(q => q.Visit)
                .ThenInclude(v => v!.PurposeType)
            .Include(q => q.Visit)
                .ThenInclude(v => v!.Status)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Token == token, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<VisitQrToken?> GetByVisitIdAsync(
        Guid visitId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .VisitQrTokens.AsNoTracking()
            .FirstOrDefaultAsync(q => q.VisitId == visitId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<VisitQrToken> AddAsync(
        VisitQrToken qrToken,
        CancellationToken cancellationToken = default
    )
    {
        qrToken.CreatedAt = DateTime.UtcNow;
        qrToken.UpdatedAt = DateTime.UtcNow;
        qrToken.IsActive = true;

        await _context.VisitQrTokens.AddAsync(qrToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Generated QR token {QrTokenId} for visit {VisitId}",
            qrToken.Id,
            qrToken.VisitId
        );
        return qrToken;
    }
}
