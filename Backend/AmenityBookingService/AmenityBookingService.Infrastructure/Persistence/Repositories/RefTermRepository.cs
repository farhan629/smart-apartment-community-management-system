using AmenityBookingService.Application.Interfaces.Repositories;
using AmenityBookingService.Domain.Entities;
using AmenityBookingService.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace AmenityBookingService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for reference term operations.
/// </summary>
public class RefTermRepository : IRefTermRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefTermRepository"/> class.
    /// </summary>
    public RefTermRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// /// Gets a reference term by its code.
    /// </summary>
    public async Task<RefTerm?> GetRefTermByCodeAsync(
        string code,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .RefTerms.AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Code == code, cancellationToken);
    }

    /// <summary>
    /// Gets a reference term by its unique identifier.
    /// </summary>
    public async Task<RefTerm?> GetRefTermByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .RefTerms.AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Id == id, cancellationToken);
    }
}
