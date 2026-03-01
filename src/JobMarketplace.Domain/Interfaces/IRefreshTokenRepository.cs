using JobMarketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Domain.Interfaces
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task RevokeAllUserTokensAsync(long userId, CancellationToken cancellationToken = default);
    }
}
