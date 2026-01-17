using MPHMS.Application.DTOs.Goals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MPHMS.Application.Services
{
    /// <summary>
    /// IGoalService defines goal management operations.
    ///
    /// Responsibilities:
    /// -----------------
    /// ✔ Create goals
    /// ✔ Update goals
    /// ✔ Track milestones
    /// ✔ Close goals
    /// ✔ Fetch user goals
    /// </summary>
    public interface IGoalService
    {
        /// <summary>
        /// Creates a new goal.
        /// </summary>
        Task<Guid> CreateGoalAsync(CreateGoalRequest request);

        /// <summary>
        /// Updates goal details.
        /// </summary>
        Task UpdateGoalAsync(Guid goalId, UpdateGoalRequest request);

        /// <summary>
        /// Soft deletes a goal.
        /// </summary>
        Task DeleteGoalAsync(Guid goalId);

        /// <summary>
        /// Returns all goals for a user.
        /// </summary>
        Task<List<GoalResponse>> GetUserGoalsAsync(Guid userId);
    }
}
