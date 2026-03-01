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
    public class DapperQueryService : IDapperQueryService
    {
        private readonly string _connectionString;

        public DapperQueryService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

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
