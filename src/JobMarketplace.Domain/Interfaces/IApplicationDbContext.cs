using JobMarketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Domain.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Company> Companies { get; }
        DbSet<Job> Jobs { get; }
        DbSet<JobApplication> JobApplications { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
