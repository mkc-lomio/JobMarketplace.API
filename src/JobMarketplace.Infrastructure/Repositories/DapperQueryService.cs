using Dapper;
using JobMarketplace.Application.Common.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace JobMarketplace.Infrastructure.Repositories
{
    /// <summary>
    /// Read side of CQRS — executes stored procedures via Dapper.
    /// Opens its own SqlConnection (separate from EF Core) so reads never
    /// interfere with write operations. Can point to a read replica later.
    /// </summary>
    public class DapperQueryService : IDapperQueryService
    {
        private readonly string _connectionString;
        private readonly ILogger<DapperQueryService> _logger;
        private const int SlowQueryThresholdMs = 300;

        public DapperQueryService(IConfiguration configuration, ILogger<DapperQueryService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _logger = logger;
        }

        private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

        /// <summary>Executes SP, returns multiple rows buffered into memory.</summary>
        public async Task<IEnumerable<T>> QueryAsync<T>(
            string storedProcedure,
            object? parameters = null,
            CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            using var connection = CreateConnection();
            var result = await connection.QueryAsync<T>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure);
            sw.Stop();

            LogQueryExecution(storedProcedure, sw.ElapsedMilliseconds);
            return result;
        }

        /// <summary>Executes SP, returns single row or null.</summary>
        public async Task<T?> QueryFirstOrDefaultAsync<T>(
            string storedProcedure,
            object? parameters = null,
            CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            using var connection = CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<T>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure);
            sw.Stop();

            LogQueryExecution(storedProcedure, sw.ElapsedMilliseconds);
            return result;
        }

        /// <summary>
        /// Streams rows one at a time — never buffers the full result set into memory.
        /// Uses Dapper's QueryUnbufferedAsync which reads from SqlDataReader row by row.
        /// The connection stays open until the caller finishes iterating.
        /// </summary>
        public async IAsyncEnumerable<T> StreamAsync<T>(
            string storedProcedure,
            object? parameters = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Streaming SP: {StoredProcedure}", storedProcedure);

            using var connection = CreateConnection();
            await connection.OpenAsync(cancellationToken);

            var stream = connection.QueryUnbufferedAsync<T>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure);

            await foreach (var item in stream.WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        /// <summary>
        /// Logs SP execution time. Warns if query exceeds threshold.
        /// </summary>
        private void LogQueryExecution(string storedProcedure, long elapsedMs)
        {
            if (elapsedMs > SlowQueryThresholdMs)
            {
                _logger.LogWarning(
                    "SLOW QUERY: {StoredProcedure} took {ElapsedMs}ms (threshold: {ThresholdMs}ms)",
                    storedProcedure, elapsedMs, SlowQueryThresholdMs);
            }
            else
            {
                _logger.LogDebug(
                    "SP: {StoredProcedure} completed in {ElapsedMs}ms",
                    storedProcedure, elapsedMs);
            }
        }
    }
}