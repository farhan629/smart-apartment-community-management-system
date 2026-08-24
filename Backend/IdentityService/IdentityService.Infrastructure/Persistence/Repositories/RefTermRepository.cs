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
    /// Repository implementation for managing reference terms in the database context.
    /// </summary>
    public class RefTermRepository : IRefTermRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefTermRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public RefTermRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RefTerm?> GetByIdAsync(Guid id)
        {
            return await _context
                .RefTerms.AsNoTracking()
                .Include(r => r.RefSet)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<RefTerm?> GetByTermValueAsync(string termValue, string setName)
        {
            return await _context
                .RefTerms.AsNoTracking()
                .Include(r => r.RefSet)
                .FirstOrDefaultAsync(r =>
                    r.Code == termValue && r.RefSet != null && r.RefSet.Code == setName
                );
        }

        public async Task<RefTerm?> GetByTermValueAndSetIdAsync(string termValue, Guid refSetId)
        {
            return await _context
                .RefTerms.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Code == termValue && r.RefSetId == refSetId);
        }

        public async Task<IEnumerable<RefTerm>> GetByRefSetIdAsync(Guid refSetId)
        {
            return await _context
                .RefTerms.AsNoTracking()
                .Where(r => r.RefSetId == refSetId && r.IsActive)
                .ToListAsync();
        }

        public async Task<RefTerm> AddAsync(RefTerm refTerm)
        {
            await _context.RefTerms.AddAsync(refTerm);
            await _context.SaveChangesAsync();
            return refTerm;
        }

        public async Task DeleteAsync(Guid id)
        {
            var term = await _context.RefTerms.FindAsync(id);
            if (term != null)
            {
                term.IsActive = false;
                term.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
