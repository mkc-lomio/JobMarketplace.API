using Dapper;
using JobMarketplace.Application.Common.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace JobMarketplace.Infrastructure.Repositories
{
    /// <summary>
    /// Read side of CQRS — executes stored procedures via Dapper.
    /// Opens its own SqlConnection (separate from EF Core) so reads never
    /// interfere with write operations. Can point to a read replica later.
    /// Interface (IDapperQueryService) lives in Application to keep Clean Architecture intact.
    /// </summary>
    public class DapperQueryService : IDapperQueryService
    {
        private readonly string _connectionString;

        public DapperQueryService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        /// <summary>Executes SP, returns multiple rows (e.g., sp_GetAllJobs).</summary>
        public async Task<IEnumerable<T>> QueryAsync<T>(
            string storedProcedure,
            object? parameters = null,
            CancellationToken cancellationToken = default)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<T>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>Executes SP, returns single row or null (e.g., sp_GetCompanyByPublicGuid).</summary>
        public async Task<T?> QueryFirstOrDefaultAsync<T>(
            string storedProcedure,
            object? parameters = null,
            CancellationToken cancellationToken = default)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure);
        }
    }
}