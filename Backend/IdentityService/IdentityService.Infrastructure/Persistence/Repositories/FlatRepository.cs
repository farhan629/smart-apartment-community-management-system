using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing flats in the database context.
    /// </summary>
    public class FlatRepository : IFlatRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlatRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public FlatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Flat?> GetByIdAsync(Guid id)
        {
            return await _context.Flats.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Flat?> GetByNumberAndBlockAsync(string number, string block)
        {
            return await _context
                .Flats.AsNoTracking()
                .FirstOrDefaultAsync(f => f.Number == number && f.Block == block);
        }

        public async Task<bool> AnyAsync()
        {
            return await _context.Flats.AnyAsync();
        }

        public async Task<Flat> AddAsync(Flat flat)
        {
            await _context.Flats.AddAsync(flat);
            await _context.SaveChangesAsync();
            return flat;
        }

        public async Task AddRangeAsync(IEnumerable<Flat> flats)
        {
            await _context.Flats.AddRangeAsync(flats);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Flat flat)
        {
            _context.Flats.Update(flat);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var flat = await _context.Flats.FindAsync(id);
            if (flat == null)
                return false;

            flat.IsActive = false;
            flat.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(int TotalCount, List<Flat> Items)> GetPagedSortedByAvailabilityAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default
        )
        {
            var query = _context
                .Flats.AsNoTracking()
                .Include(f => f.FlatOccupancies)
                .Where(f => f.IsActive)
                .OrderBy(f => f.FlatOccupancies!.Any(o => o.IsActive && o.IsApproved) ? 1 : 0)
                .ThenBy(f => f.Block)
                .ThenBy(f => f.Number);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (totalCount, items);
        }

        public async Task<Flat?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            return await _context
                .Flats.AsNoTracking()
                .Include(f => f.FlatOccupancies)
                .FirstOrDefaultAsync(f => f.Id == id && f.IsActive, cancellationToken);
        }
    }
}
