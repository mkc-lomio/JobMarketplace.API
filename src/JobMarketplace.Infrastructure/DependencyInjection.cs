using JobMarketplace.Domain.Interfaces;
using JobMarketplace.Infrastructure.Persistence;
using JobMarketplace.Infrastructure.Persistence.Interceptors;
using JobMarketplace.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using JobMarketplace.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Infrastructure
{
    /// <summary>
    /// Registers all Infrastructure services — maps every interface to its concrete implementation.
    /// When a handler asks for ICompanyRepository, DI gives it CompanyRepository.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            // Audit interceptor — auto-stamps CreatedAt/LastModifiedAt on every SaveChanges
            services.AddSingleton<AuditableEntityInterceptor>();

            // EF Core DbContext — SQL Server + audit interceptor
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();

                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

                options.AddInterceptors(interceptor);
            });

            // Interface → concrete mapping for DbContext
            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<ApplicationDbContext>());

            // Repositories — EF Core, used by Command handlers (writes)
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();

            // Dapper — separate SqlConnection, used by Query handlers (reads via stored procedures)
            services.AddScoped<IDapperQueryService, DapperQueryService>();

            return services;
        }
    }
}