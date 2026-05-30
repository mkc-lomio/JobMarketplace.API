using JobMarketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Domain.Interfaces
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        Task<string> GetCountryCode(long id, CancellationToken cancellationToken = default);
    }
}
