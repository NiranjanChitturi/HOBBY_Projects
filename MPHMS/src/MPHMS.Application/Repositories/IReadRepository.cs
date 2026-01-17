using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MPHMS.Application.Repositories
{
    /// <summary>
    /// IReadRepository defines READ-ONLY database access.
    ///
    /// Why this exists:
    /// ----------------
    /// Separating READ and WRITE responsibilities follows:
    /// - CQRS principles (Command Query Responsibility Segregation)
    /// - Improves performance tuning
    /// - Prevents accidental data modification
    ///
    /// This interface is implemented by Infrastructure layer.
    ///
    /// Application layer NEVER talks directly to EF Core.
    /// </summary>
    /// <typeparam name="TEntity">
    /// Domain entity type.
    /// Example:
    /// Habit, Goal, UserProfile
    /// </typeparam>
    public interface IReadRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Fetch entity by Primary Key.
        ///
        /// Example:
        /// --------
        /// Get habit by HabitId
        /// </summary>
        Task<TEntity?> GetByIdAsync(Guid id);

        /// <summary>
        /// Returns ALL records of this entity.
        ///
        /// WARNING:
        /// --------
        /// Should be used carefully on large tables.
        /// Prefer pagination for production use.
        /// </summary>
        Task<IReadOnlyList<TEntity>> GetAllAsync();

        /// <summary>
        /// Finds entities matching a condition.
        ///
        /// Example:
        /// --------
        /// Find all active habits:
        /// h => h.IsActive == true
        /// </summary>
        Task<IReadOnlyList<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Checks if any record exists matching condition.
        ///
        /// Example:
        /// --------
        /// Does user already have this habit?
        /// </summary>
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Returns count of records matching condition.
        ///
        /// Used for:
        /// --------
        /// - Analytics
        /// - Dashboard KPIs
        /// - Pagination
        /// </summary>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
