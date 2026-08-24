using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Domain.Entities;
using ComplaintMaintenanceService.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComplaintMaintenanceService.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for managing reference sets in the database context.
    /// </summary>
    public class RefSetRepository : IRefSetRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RefSetRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefSetRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RefSetRepository(AppDbContext context, ILogger<RefSetRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RefSet?> GetByCodeAsync(string code)
        {
            return await _context.RefSets.FirstOrDefaultAsync(r => r.Code == code);
        }

        public async Task<RefSet?> GetByIdAsync(Guid id)
        {
            return await _context.RefSets.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<RefSet> AddAsync(RefSet refSet)
        {
            await _context.RefSets.AddAsync(refSet);
            await _context.SaveChangesAsync();
            return refSet;
        }

        public async Task UpdateAsync(RefSet refSet)
        {
            _context.RefSets.Update(refSet);
            await _context.SaveChangesAsync();
        }
    }
}