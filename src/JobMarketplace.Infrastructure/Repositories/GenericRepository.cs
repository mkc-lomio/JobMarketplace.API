using JobMarketplace.Domain.Common;
using JobMarketplace.Domain.Interfaces;
using JobMarketplace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobMarketplace.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of IGenericRepository — write side of CQRS.
    /// Handles CRUD for any entity. Entity-specific repos inherit this and add specialized methods.
    /// Protected _context/_dbSet so child repos (JobRepository, etc.) can use them for custom queries.
    /// </summary>
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseAuditableEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        public async Task<T?> GetByPublicGuidAsync(Guid publicGuid, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(e => e.PublicGuid == publicGuid, cancellationToken);

        // AsNoTracking — skips change tracking since we're only reading, not modifying
        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
            => await _dbSet.AddAsync(entity, cancellationToken);

        public void Update(T entity)
            => _dbSet.Update(entity);

        public void Remove(T entity)
            => _dbSet.Remove(entity);

        // Unit-of-work: commits all staged Add/Update/Remove in one transaction
        // AuditableEntityInterceptor fires here to stamp CreatedAt/LastModifiedAt
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);
    }
}