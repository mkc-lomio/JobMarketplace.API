using JobMarketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Domain.Interfaces
{
    public interface IJobRepository : IGenericRepository<Job>
    {
        Task<Job?> GetActiveJobByPublicGuidAsync(Guid publicGuid, CancellationToken cancellationToken = default);
    }
}
