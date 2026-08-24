using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Domain.Entities;
using ComplaintMaintenanceService.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComplaintMaintenanceService.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for managing reference terms in the database context.
    /// </summary>
    public class RefTermRepository : IRefTermRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RefTermRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefTermRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RefTermRepository(AppDbContext context, ILogger<RefTermRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RefTerm?> GetByIdAsync(Guid id)
        {
            return await _context.RefTerms.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<RefTerm?> GetByCodeAsync(string code) =>
            await _context.RefTerms.FirstOrDefaultAsync(r => r.Code == code);

        public async Task<RefTerm?> GetByCodeAndSetIdAsync(string code, Guid refSetId)
        {
            return await _context.RefTerms.FirstOrDefaultAsync(r =>
                r.Code == code && r.RefSetId == refSetId
            );
        }

        public async Task<IEnumerable<RefTerm>> GetByRefSetIdAsync(Guid refSetId)
        {
            return await _context.RefTerms.Where(r => r.RefSetId == refSetId).ToListAsync();
        }

        public async Task<RefTerm> AddAsync(RefTerm refTerm)
        {
            await _context.RefTerms.AddAsync(refTerm);
            await _context.SaveChangesAsync();
            return refTerm;
        }

        public async Task UpdateAsync(RefTerm refTerm)
        {
            _context.RefTerms.Update(refTerm);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var term = await _context.RefTerms.FirstOrDefaultAsync(r => r.Id == id);
            if (term is not null)
            {
                _context.RefTerms.Remove(term);
                await _context.SaveChangesAsync();
            }
        }
    }
}