using MPHMS.Application.Repositories;
using MPHMS.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace MPHMS.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// UnitOfWork coordinates ALL database operations.
    ///
    /// Purpose:
    /// --------
    /// Acts as a TRANSACTION BOUNDARY.
    ///
    /// Ensures multiple repository changes are:
    /// ✔ Committed together
    /// ✔ Rolled back together on failure
    ///
    /// Enterprise Pattern:
    /// -------------------
    /// Unit Of Work Pattern
    /// Repository Pattern
    /// Transaction Boundary Pattern
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor injection of DbContext.
        /// </summary>
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Saves ALL pending changes to database.
        ///
        /// This method fulfills IUnitOfWork contract.
        ///
        /// IMPORTANT:
        /// ----------
        /// Audit fields and soft deletes are automatically handled
        /// by ApplicationDbContext.SaveChangesAsync override.
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Releases database resources.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
