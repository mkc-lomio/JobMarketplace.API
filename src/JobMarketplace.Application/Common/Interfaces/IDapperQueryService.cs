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
    }
}
