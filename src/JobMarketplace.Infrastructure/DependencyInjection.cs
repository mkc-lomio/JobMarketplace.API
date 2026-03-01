using JobMarketplace.Application.Common.Interfaces;
using JobMarketplace.Domain.Interfaces;
using JobMarketplace.Infrastructure.Persistence;
using JobMarketplace.Infrastructure.Persistence.Interceptors;
using JobMarketplace.Infrastructure.Repositories;
using JobMarketplace.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            // HttpContextAccessor — needed by AuditableEntityInterceptor to read JWT claims
            services.AddHttpContextAccessor();

            // Audit interceptor — auto-stamps CreatedAt/LastModifiedAt on every SaveChanges
            services.AddScoped<AuditableEntityInterceptor>();

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
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // Dapper — separate SqlConnection, used by Query handlers (reads via stored procedures)
            services.AddScoped<IDapperQueryService, DapperQueryService>();

            // Auth services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            // Health checks — SQL Server connectivity
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddHealthChecks()
                .AddSqlServer(connectionString, name: "sqlserver", tags: ["db", "ready"]);

            return services;
        }
    }
}