using System;
using System.Threading.Tasks;

namespace MPHMS.Application.Repositories
{
    /// <summary>
    /// IUnitOfWork manages database transactions.
    ///
    /// Why Unit Of Work:
    /// -----------------
    /// Example scenario:
    ///
    /// Create Habit
    /// Create Habit Schedule
    /// Create Habit Reminder
    ///
    /// ALL must succeed or ALL must fail.
    ///
    /// This interface guarantees:
    /// --------------------------
    /// - Atomic operations
    /// - Data consistency
    /// - Transaction safety
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Saves all pending changes to database.
        ///
        /// Returns:
        /// --------
        /// Number of affected rows.
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
