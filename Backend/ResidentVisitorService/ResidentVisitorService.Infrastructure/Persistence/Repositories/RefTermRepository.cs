using Microsoft.EntityFrameworkCore;
using ResidentVisitorService.Application.Interfaces.Repositories;
using ResidentVisitorService.Domain.Entities;
using ResidentVisitorService.Infrastructure.Persistence.DBContext;

namespace ResidentVisitorService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for <see cref="RefTerm"/> lookup data access.
/// </summary>
public class RefTermRepository : IRefTermRepository
{
    private readonly AppDbContext _context;

    public RefTermRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<RefTerm>> GetByRefSetCodeAsync(
        string refSetCode,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .RefTerms.Where(t =>
                t.RefSet != null && t.RefSet.Code == refSetCode && t.IsActive && t.RefSet.IsActive
            )
            .AsNoTracking()
            .OrderBy(t => t.DisplayName)
            .Select(t => new RefTerm
            {
                Id = t.Id,
                Code = t.Code,
                DisplayName = t.DisplayName,
                RefSetId = t.RefSetId,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                CreatedBy = t.CreatedBy,
                UpdatedAt = t.UpdatedAt,
                UpdatedBy = t.UpdatedBy,
            })
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<RefTerm?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context
            .RefTerms.Where(t => t.Id == id && t.IsActive)
            .AsNoTracking()
            .Select(t => new RefTerm
            {
                Id = t.Id,
                Code = t.Code,
                DisplayName = t.DisplayName,
                RefSetId = t.RefSetId,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                CreatedBy = t.CreatedBy,
                UpdatedAt = t.UpdatedAt,
                UpdatedBy = t.UpdatedBy,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<RefTerm?> GetByCodeAsync(
        string refSetCode,
        string termCode,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .RefTerms.Where(t =>
                t.RefSet != null && t.RefSet.Code == refSetCode && t.Code == termCode && t.IsActive
            )
            .AsNoTracking()
            .Select(t => new RefTerm
            {
                Id = t.Id,
                Code = t.Code,
                DisplayName = t.DisplayName,
                RefSetId = t.RefSetId,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                CreatedBy = t.CreatedBy,
                UpdatedAt = t.UpdatedAt,
                UpdatedBy = t.UpdatedBy,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
