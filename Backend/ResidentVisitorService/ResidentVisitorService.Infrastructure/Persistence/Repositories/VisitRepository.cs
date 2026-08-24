using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Interfaces.Repositories;
using ResidentVisitorService.Domain.Entities;
using ResidentVisitorService.Infrastructure.Persistence.DBContext;

namespace ResidentVisitorService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for <see cref="Visit"/> data access.
/// </summary>
public class VisitRepository : IVisitRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<VisitRepository> _logger;

    public VisitRepository(AppDbContext context, ILogger<VisitRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Visit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context
            .Visits.Include(v => v.Visitor)
                .ThenInclude(vis => vis!.VisitorType)
            .Include(v => v.PurposeType)
            .Include(v => v.Status)
            .Include(v => v.VisitQrToken)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id && v.IsActive, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<(int TotalCount, List<Visit> Items)> GetAllAsync(
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
    )
    {
        IQueryable<Visit> query = _context
            .Visits.Include(v => v.Visitor)
                .ThenInclude(vis => vis!.VisitorType)
            .Include(v => v.PurposeType)
            .Include(v => v.Status)
            .Include(v => v.VisitQrToken)
            .Where(v => v.IsActive)
            .AsNoTracking();

        if (visitorId.HasValue)
        {
            query = query.Where(v => v.VisitorId == visitorId.Value);
        }

        if (hostUserId.HasValue)
        {
            query = query.Where(v => v.HostUserId == hostUserId.Value);
        }

        if (flatId.HasValue)
        {
            query = query.Where(v => v.FlatId == flatId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var upperStatus = status.ToUpper().Trim();
            query = query.Where(v => v.Status != null && v.Status.Code == upperStatus);
        }

        if (startDate.HasValue)
        {
            var startDateTime = startDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            query = query.Where(v => v.StartDate >= startDateTime);
        }

        if (endDate.HasValue)
        {
            var endDateTime = endDate.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
            query = query.Where(v => v.EndDate <= endDateTime);
        }

        query = sortBy.ToLower() switch
        {
            "startdate" => sortOrder != ResidentVisitorConstants.Pagination.DefaultSortOrder
                ? query.OrderBy(v => v.StartDate)
                : query.OrderByDescending(v => v.StartDate),
            "enddate" => sortOrder != ResidentVisitorConstants.Pagination.DefaultSortOrder
                ? query.OrderBy(v => v.EndDate)
                : query.OrderByDescending(v => v.EndDate),
            "status" => sortOrder != ResidentVisitorConstants.Pagination.DefaultSortOrder
                ? query.OrderBy(v => v.Status!.Code)
                : query.OrderByDescending(v => v.Status!.Code),
            _ => sortOrder != ResidentVisitorConstants.Pagination.DefaultSortOrder
                ? query.OrderBy(v => v.CreatedAt)
                : query.OrderByDescending(v => v.CreatedAt),
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(cancellationToken);

        return (totalCount, items);
    }

    /// <inheritdoc/>
    public async Task<Visit> AddAsync(Visit visit, CancellationToken cancellationToken = default)
    {
        visit.CreatedAt = DateTime.UtcNow;
        visit.UpdatedAt = DateTime.UtcNow;
        visit.IsActive = true;

        await _context.Visits.AddAsync(visit, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Inserted visit {VisitId}", visit.Id);
        return visit;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Visit visit, CancellationToken cancellationToken = default)
    {
        visit.UpdatedAt = DateTime.UtcNow;
        _context.Entry(visit).State = EntityState.Modified;
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var visit = await _context.Visits.FirstOrDefaultAsync(
            v => v.Id == id && v.IsActive,
            cancellationToken
        );

        if (visit is null)
        {
            return false;
        }

        visit.IsActive = false;
        visit.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Soft-deleted (cancelled) visit {VisitId}", id);
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> HasActiveVisitAsync(
        Guid visitorId,
        Guid hostUserId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default
    )
    {
        var startUtc = startDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var endUtc = endDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        return await _context
            .Visits.AsNoTracking()
            .AnyAsync(
                v =>
                    v.VisitorId == visitorId
                    && v.HostUserId == hostUserId
                    && v.IsActive
                    && v.Status != null
                    && v.Status.Code != ResidentVisitorConstants.VisitStatus.REJECTED
                    && v.Status.Code != ResidentVisitorConstants.VisitStatus.CHECKED_OUT
                    && v.StartDate <= endUtc
                    && v.EndDate >= startUtc,
                cancellationToken
            );
    }
}
