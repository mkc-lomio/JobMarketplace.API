using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Application.Common.Interfaces
{
    public interface IDapperQueryService
    {
        Task<IEnumerable<T>> QueryAsync<T>(
            string storedProcedure,
            object? parameters = null,
            CancellationToken cancellationToken = default);

        Task<T?> QueryFirstOrDefaultAsync<T>(
            string storedProcedure,
            object? parameters = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Streams rows one at a time via IAsyncEnumerable — never buffers the full result set.
        /// Uses Dapper's QueryUnbufferedAsync under the hood.
        /// Why? For large result sets, this can reduce memory usage 
        /// and latency by processing rows as they arrive instead of waiting for the entire set. 
        /// The caller can consume the stream with await foreach and apply backpressure if needed.
        /// </summary>
        IAsyncEnumerable<T> StreamAsync<T>(
            string storedProcedure,
            object? parameters = null,
            CancellationToken cancellationToken = default);
    }
}
