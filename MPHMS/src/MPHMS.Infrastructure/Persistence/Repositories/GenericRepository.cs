using Microsoft.EntityFrameworkCore;
using MPHMS.Application.Repositories;
using MPHMS.Infrastructure.Persistence;
using System.Threading.Tasks;

namespace MPHMS.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// GenericRepository provides WRITE operations.
    ///
    /// Responsibilities:
    /// -----------------
    /// ✔ Insert
    /// ✔ Update
    /// ✔ Delete (Soft Delete)
    ///
    /// NOTE:
    /// -----
    /// Actual database commit is handled ONLY by UnitOfWork.
    /// </summary>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Constructor injection.
        /// </summary>
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Adds entity to ChangeTracker.
        /// Does NOT hit database until SaveChangesAsync.
        /// </summary>
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        /// <summary>
        /// Updates entity state.
        /// </summary>
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <summary>
        /// Removes entity.
        ///
        /// IMPORTANT:
        /// ----------
        /// If entity inherits BaseAuditableEntity
        /// Soft delete will be applied automatically
        /// by ApplicationDbContext interceptor.
        /// </summary>
        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
