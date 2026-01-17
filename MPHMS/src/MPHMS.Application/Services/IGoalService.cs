using MPHMS.Application.DTOs.Goals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MPHMS.Application.Services
{
    /// <summary>
    /// Contract for Goal business operations.
    ///
    /// This interface defines ALL application-level
    /// goal-related use cases.
    ///
    /// API Layer depends ONLY on this abstraction.
    /// </summary>
    public interface IGoalService
    {
        // ------------------------
        // Goal Management
        // ------------------------

        Task<Guid> CreateGoalAsync(CreateGoalRequest request);

        Task UpdateGoalAsync(Guid goalId, UpdateGoalRequest request);

        Task DeleteGoalAsync(Guid goalId);

        Task<List<GoalResponse>> GetUserGoalsAsync(Guid userId);

        // ------------------------
        // Milestone Management
        // ------------------------
        Task<List<GoalResponse>> GetMyGoalsAsync();


        Task AddMilestoneAsync(AddMilestoneRequest request);

        Task UpdateMilestoneProgressAsync(UpdateMilestoneProgressRequest request);
    }
}
