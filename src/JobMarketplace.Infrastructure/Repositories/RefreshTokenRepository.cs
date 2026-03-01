using JobMarketplace.Domain.Entities;
using JobMarketplace.Domain.Interfaces;
using JobMarketplace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Infrastructure.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context) { }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
            => await _dbSet.Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);

        /// <summary>
        /// Revokes ALL active tokens for a user — called when a revoked token is reused (theft detection).
        /// </summary>
        public async Task RevokeAllUserTokensAsync(long userId, CancellationToken cancellationToken = default)
        {
            var activeTokens = await _dbSet
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync(cancellationToken);

            foreach (var token in activeTokens)
                token.RevokedAt = DateTime.UtcNow;
        }
    }
}
