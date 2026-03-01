using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Domain.Interfaces
{
    /// <summary>
    /// Generic CRUD contract. Lives in Domain (not Infrastructure) because of the
    /// Dependency Inversion Principle: inner layers define contracts, outer layers implement.
    /// Application depends on this interface. Infrastructure provides GenericRepository&lt;T&gt;.
    /// Swap EF Core for anything else — only Infrastructure changes.
    /// </summary>
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);           // Lookup by internal BIGINT
        Task<T?> GetByPublicGuidAsync(Guid publicGuid, CancellationToken cancellationToken = default); // Lookup by external GUID
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);   // Stages INSERT
        void Update(T entity);                                                     // Stages UPDATE
        void Remove(T entity);                                                     // Stages DELETE
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); // Commits all staged changes in one transaction
    }
}