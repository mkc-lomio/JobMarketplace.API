using JobMarketplace.Domain.Entities;
using JobMarketplace.Domain.Interfaces;
using JobMarketplace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Infrastructure.Repositories
{
    public class CountryRepository : GenericRepository<Country>, ICountryRepository
    {
         public CountryRepository(ApplicationDbContext context) : base(context) { }


        public async Task<string> GetCountryCode(long id, CancellationToken cancellationToken = default)
        {
            var country = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            return country.Name ?? string.Empty;
        }
    }
}
