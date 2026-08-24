using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityService.Application.Features.Users.DTOs;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing flat occupancies in the database context.
    /// </summary>
    public class FlatOccupancyRepository : IFlatOccupancyRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlatOccupancyRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public FlatOccupancyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasPendingRequestAsync(Guid flatId)
        {
            return await _context.FlatOccupancies.AnyAsync(o =>
                o.FlatId == flatId && !o.IsApproved && o.IsActive
            );
        }

        public async Task<FlatOccupancy> AddAsync(FlatOccupancy occupancy)
        {
            await _context.FlatOccupancies.AddAsync(occupancy);
            await _context.SaveChangesAsync();
            return occupancy;
        }

        public async Task<FlatOccupancy?> GetByIdAsync(Guid id)
        {
            return await _context
                .FlatOccupancies.AsNoTracking()
                .Where(o => o.Id == id)
                .Select(o => new FlatOccupancy
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    User =
                        o.User != null
                            ? new User
                            {
                                Id = o.User.Id,
                                Name = o.User.Name,
                                Email = o.User.Email,
                            }
                            : null,
                    FlatId = o.FlatId,
                    Flat =
                        o.Flat != null
                            ? new Flat
                            {
                                Id = o.Flat.Id,
                                Number = o.Flat.Number,
                                Block = o.Flat.Block,
                            }
                            : null,
                    ResidentTypeId = o.ResidentTypeId,
                    ResidentType =
                        o.ResidentType != null
                            ? new RefTerm
                            {
                                Id = o.ResidentType.Id,
                                DisplayName = o.ResidentType.DisplayName,
                            }
                            : null,
                    IsApproved = o.IsApproved,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    IsActive = o.IsActive,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(int Total, IEnumerable<FlatOccupancy> Items)> GetAllAsync(
            int page,
            int limit,
            string? status,
            Guid? userId
        )
        {
            var query = _context
                .FlatOccupancies.AsNoTracking()
                .Where(o => o.IsActive)
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(o => o.UserId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status.Equals("pending", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(o => !o.IsApproved);
                }
                else if (status.Equals("approved", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(o => o.IsApproved);
                }
            }

            query = query.OrderByDescending(o => o.CreatedAt);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(o => new FlatOccupancy
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    User =
                        o.User != null
                            ? new User
                            {
                                Id = o.User.Id,
                                Name = o.User.Name,
                                Email = o.User.Email,
                            }
                            : null,
                    FlatId = o.FlatId,
                    Flat =
                        o.Flat != null
                            ? new Flat
                            {
                                Id = o.Flat.Id,
                                Number = o.Flat.Number,
                                Block = o.Flat.Block,
                            }
                            : null,
                    ResidentTypeId = o.ResidentTypeId,
                    ResidentType =
                        o.ResidentType != null
                            ? new RefTerm
                            {
                                Id = o.ResidentType.Id,
                                DisplayName = o.ResidentType.DisplayName,
                            }
                            : null,
                    IsApproved = o.IsApproved,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    IsActive = o.IsActive,
                })
                .ToListAsync();

            return (total, items);
        }

        public async Task UpdateAsync(FlatOccupancy occupancy)
        {
            _context.FlatOccupancies.Update(occupancy);
            await _context.SaveChangesAsync();
        }

        public async Task<FlatOccupancy?> GetActiveOccupancyByFlatAndRoleAsync(
            Guid flatId,
            Guid residentTypeId
        )
        {
            return await _context
                .FlatOccupancies.AsNoTracking()
                .FirstOrDefaultAsync(o =>
                    o.FlatId == flatId && o.ResidentTypeId == residentTypeId && o.IsActive
                );
        }

        public async Task<Guid?> getUserIdFlat(Guid userId)
        {
            Guid? Id = await (
                from f in _context.FlatOccupancies
                where f.UserId == userId
                select (Guid?)f.FlatId
            ).FirstOrDefaultAsync();
            return Id;
        }
    }
}
