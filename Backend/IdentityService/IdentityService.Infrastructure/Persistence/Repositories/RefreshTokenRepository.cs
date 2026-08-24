using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetActiveByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default
        )
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(
                r => r.UserId == userId && r.IsActive,
                cancellationToken
            );
        }

        public async Task<RefreshToken?> GetByTokenKeyAsync(
            string tokenKey,
            CancellationToken cancellationToken = default
        )
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(
                r => r.TokenKey == tokenKey,
                cancellationToken
            );
        }

        /// <summary>
        /// If the user already has an active refresh token row, replace its TokenKey/ExpiryAt (and bump UpdatedAt/UpdatedBy).
        /// Otherwise create a brand new row. Ensures one active refresh token per user at any time.
        /// </summary>
        public async Task UpsertAsync(
            Guid userId,
            string tokenKey,
            DateTime expiryAt,
            Guid? performedBy,
            CancellationToken cancellationToken = default
        )
        {
            var existing = await _context.RefreshTokens.FirstOrDefaultAsync(
                r => r.UserId == userId && r.IsActive,
                cancellationToken
            );

            if (existing is null)
            {
                var entity = new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TokenKey = tokenKey,
                    ExpiryAt = expiryAt,
                };
                await _context.RefreshTokens.AddAsync(entity, cancellationToken);
            }
            else
            {
                existing.TokenKey = tokenKey;
                existing.ExpiryAt = expiryAt;

                _context.RefreshTokens.Update(existing);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeactivateAsync(
            Guid userId,
            Guid? performedBy,
            CancellationToken cancellationToken = default
        )
        {
            var existing = await _context.RefreshTokens.FirstOrDefaultAsync(
                r => r.UserId == userId && r.IsActive,
                cancellationToken
            );

            if (existing is null)
                return;

            existing.IsActive = false;
            _context.RefreshTokens.Update(existing);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
