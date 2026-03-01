using JobMarketplace.Domain.Interfaces;
using JobMarketplace.Infrastructure.Persistence;
using JobMarketplace.Infrastructure.Persistence.Interceptors;
using JobMarketplace.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using JobMarketplace.Application.Common.Interfaces;

namespace JobMarketplace.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<AuditableEntityInterceptor>();

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();

                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

                options.AddInterceptors(interceptor);
            });

            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<ApplicationDbContext>());

            // Repositories (EF Core — for Commands/writes)
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();

            // Dapper Query Service (for Queries/reads via Stored Procedures)
            services.AddScoped<IDapperQueryService, DapperQueryService>();

            return services;
        }
    }
}
