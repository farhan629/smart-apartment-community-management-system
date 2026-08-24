using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Interfaces.Repositories;
using ResidentVisitorService.Domain.Entities;
using ResidentVisitorService.Infrastructure.Persistence.DBContext;

namespace ResidentVisitorService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for <see cref="Visitor"/> data access.
/// </summary>
public class VisitorRepository : IVisitorRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<VisitorRepository> _logger;

    public VisitorRepository(AppDbContext context, ILogger<VisitorRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Visitor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context
            .Visitors.Include(v => v.VisitorType)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id && v.IsActive, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Visitor?> GetByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Visitors.Include(v => v.VisitorType)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                v => v.PhoneNumber == phoneNumber && v.IsActive,
                cancellationToken
            );
    }

    /// <inheritdoc/>
    public async Task<(int TotalCount, List<Visitor> Items)> GetAllAsync(
        string? search,
        string sortBy,
        string sortOrder,
        int page,
        int limit,
        Guid? hostUserId = null,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<Visitor> query = _context
            .Visitors.Include(v => v.VisitorType)
            .Where(v => v.IsActive);

        if (hostUserId.HasValue)
        {
            query = query.Where(v =>
                _context.Visits.Any(vi => vi.VisitorId == v.Id && vi.HostUserId == hostUserId.Value)
            );
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalised = search.ToLower().Trim();
            query = query.Where(v =>
                v.Name.ToLower().Contains(normalised) || v.PhoneNumber.Contains(normalised)
            );
        }

        query = sortBy.ToLower() switch
        {
            "name" => sortOrder == ResidentVisitorConstants.Pagination.DefaultSortOrder
                ? query.OrderByDescending(v => v.Name)
                : query.OrderBy(v => v.Name),
            "phonenumber" => sortOrder == ResidentVisitorConstants.Pagination.DefaultSortOrder
                ? query.OrderByDescending(v => v.PhoneNumber)
                : query.OrderBy(v => v.PhoneNumber),
            _ => sortOrder == ResidentVisitorConstants.Pagination.DefaultSortOrder
                ? query.OrderByDescending(v => v.CreatedAt)
                : query.OrderBy(v => v.CreatedAt),
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .AsNoTracking()
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return (totalCount, items);
    }

    /// <inheritdoc/>
    public async Task<Visitor> AddAsync(
        Visitor visitor,
        CancellationToken cancellationToken = default
    )
    {
        visitor.CreatedAt = DateTime.UtcNow;
        visitor.UpdatedAt = DateTime.UtcNow;
        visitor.IsActive = true;

        await _context.Visitors.AddAsync(visitor, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Inserted visitor {VisitorId}", visitor.Id);
        return visitor;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Visitor visitor, CancellationToken cancellationToken = default)
    {
        visitor.UpdatedAt = DateTime.UtcNow;
        _context.Visitors.Update(visitor);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated visitor {VisitorId}", visitor.Id);
    }

    /// <inheritdoc/>
    public async Task UpdatePhotoUrlAsync(
        Guid id,
        string photoUrl,
        CancellationToken cancellationToken = default
    )
    {
        var visitor =
            await _context.Visitors.FirstOrDefaultAsync(
                v => v.Id == id && v.IsActive,
                cancellationToken
            )
            ?? throw new KeyNotFoundException(
                string.Format(ResidentVisitorConstants.Errors.VisitorNotFound, id)
            );

        visitor.PhotoUrl = photoUrl;
        visitor.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated photo URL for visitor {VisitorId}", id);
    }

    /// <inheritdoc/>
    public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var visitor = await _context.Visitors.FirstOrDefaultAsync(
            v => v.Id == id && v.IsActive,
            cancellationToken
        );

        if (visitor is null)
        {
            return false;
        }

        visitor.IsActive = false;
        visitor.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Soft-deleted visitor {VisitorId}", id);
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> PhoneNumberExistsAsync(
        string phoneNumber,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Visitors.AsNoTracking()
            .AnyAsync(
                v =>
                    v.PhoneNumber == phoneNumber
                    && v.IsActive
                    && (excludeId == null || v.Id != excludeId),
                cancellationToken
            );
    }
}
