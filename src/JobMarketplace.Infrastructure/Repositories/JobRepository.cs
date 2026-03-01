using JobMarketplace.Domain.Entities;
using JobMarketplace.Domain.Enums;
using JobMarketplace.Domain.Interfaces;
using JobMarketplace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Infrastructure.Repositories
{
    public class JobRepository : GenericRepository<Job>, IJobRepository
    {
        public JobRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Job?> GetActiveJobByPublicGuidAsync(
            Guid publicGuid, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(j => j.PublicGuid == publicGuid
                    && j.Status == JobStatus.Active, cancellationToken);
        }
    }
}
