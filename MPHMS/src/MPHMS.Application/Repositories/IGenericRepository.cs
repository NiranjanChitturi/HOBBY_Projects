using System;
using System.Threading.Tasks;

namespace MPHMS.Application.Repositories
{
    /// <summary>
    /// IGenericRepository defines WRITE operations.
    ///
    /// This repository:
    /// ----------------
    /// - Does NOT save immediately
    /// - Works with Unit Of Work
    ///
    /// This ensures:
    /// --------------
    /// - Transaction consistency
    /// - Better performance
    /// - Atomic operations
    /// </summary>
    /// <typeparam name="TEntity">
    /// Domain entity type.
    /// Example:
    /// Habit, Goal, HabitLog
    /// </typeparam>
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Adds new entity to DbContext change tracker.
        ///
        /// Actual database save happens
        /// ONLY when UnitOfWork.SaveChangesAsync() is called.
        /// </summary>
        Task AddAsync(TEntity entity);

        /// <summary>
        /// Updates existing entity.
        ///
        /// EF Core will automatically detect changes.
        /// </summary>
        void Update(TEntity entity);

        /// <summary>
        /// Removes entity.
        ///
        /// IMPORTANT:
        /// ----------
        /// Because we implemented SOFT DELETE,
        /// this usually marks IsDeleted = true.
        /// </summary>
        void Remove(TEntity entity);
    }
}
