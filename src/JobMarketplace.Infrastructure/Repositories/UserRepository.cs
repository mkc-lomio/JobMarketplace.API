using JobMarketplace.Domain.Entities;
using JobMarketplace.Domain.Interfaces;
using JobMarketplace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => await _dbSet.Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
            => await _dbSet.AnyAsync(u => u.Email == email, cancellationToken);
    }
}
