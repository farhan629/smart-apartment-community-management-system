using System.Threading.Tasks;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing reference sets in the database context.
    /// </summary>
    public class RefSetRepository : IRefSetRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefSetRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public RefSetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RefSet?> GetBySetNameAsync(string setName)
        {
            return await _context
                .RefSets.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Code == setName);
        }

        public async Task<RefSet> AddAsync(RefSet refSet)
        {
            await _context.RefSets.AddAsync(refSet);
            await _context.SaveChangesAsync();
            return refSet;
        }
    }
}
