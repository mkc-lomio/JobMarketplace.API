using JobMarketplace.Domain.Entities;
using JobMarketplace.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace JobMarketplace.Infrastructure.Persistence
{
    /// <summary>
    /// EF Core DbContext — single gateway to the database for write operations.
    /// Implements IApplicationDbContext (Domain interface) so inner layers don't depend on EF Core.
    /// Read operations bypass this entirely — they go through DapperQueryService.
    /// </summary>
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Auto-discovers all IEntityTypeConfiguration<T> in this assembly
            // (CompanyConfiguration, JobConfiguration, JobApplicationConfiguration)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}