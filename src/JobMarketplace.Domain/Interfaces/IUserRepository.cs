using JobMarketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Domain.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}
