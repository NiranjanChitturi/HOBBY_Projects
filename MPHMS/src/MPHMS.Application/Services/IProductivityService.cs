using System;
using System.Threading.Tasks;

namespace MPHMS.Application.Services
{
    /// <summary>
    /// IProductivityService handles analytics and scoring.
    ///
    /// Phase 1 Scope:
    /// --------------
    /// ✔ Monthly productivity score calculation
    /// ✔ Snapshot persistence
    ///
    /// Later Phases:
    /// -------------
    /// - AI insights
    /// - Behavior prediction
    /// - Pattern clustering
    /// </summary>
    public interface IProductivityService
    {
        /// <summary>
        /// Calculates monthly productivity score
        /// for a user and persists snapshot.
        /// </summary>
        Task CalculateMonthlyScoreAsync(Guid userId, int year, int month);
    }
}
