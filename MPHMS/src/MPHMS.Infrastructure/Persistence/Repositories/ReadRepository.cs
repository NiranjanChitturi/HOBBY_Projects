using Microsoft.EntityFrameworkCore;
using MPHMS.Application.Repositories;
using MPHMS.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MPHMS.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// ReadRepository provides READ-ONLY data access logic.
    ///
    /// Purpose:
    /// --------
    /// - Optimized query execution
    /// - No accidental write operations
    /// - CQRS read side implementation
    ///
    /// Application layer depends ONLY on IReadRepository interface.
    /// Infrastructure provides concrete EF Core implementation.
    /// </summary>
    public class ReadRepository<T> : IReadRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Constructor.
        /// Receives DbContext from Dependency Injection.
        /// </summary>
        public ReadRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Returns entity by primary key (GUID).
        ///
        /// Uses EF Core FindAsync:
        /// ✔ Checks change tracker first
        /// ✔ Falls back to database if needed
        /// </summary>
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Returns all records.
        ///
        /// Soft delete filter automatically applied.
        /// Uses AsNoTracking for performance.
        /// </summary>
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Finds entities matching condition.
        ///
        /// Example:
        /// FindAsync(x => x.IsActive)
        /// </summary>
        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync();
        }

        /// <summary>
        /// Returns total count matching condition.
        /// </summary>
        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        /// <summary>
        /// Checks existence of record.
        /// </summary>
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}
