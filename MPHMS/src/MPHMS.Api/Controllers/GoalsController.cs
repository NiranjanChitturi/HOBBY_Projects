using Microsoft.AspNetCore.Mvc;
using MPHMS.Api.Controllers.Base;
using MPHMS.Application.DTOs.Goals;
using MPHMS.Application.Services;
using System;
using System.Threading.Tasks;

namespace MPHMS.Api.Controllers
{
    /// <summary>
    /// GoalsController exposes REST endpoints
    /// for Goal and Milestone management.
    ///
    /// Responsibilities:
    /// -----------------
    /// ✔ Create goal
    /// ✔ Update goal
    /// ✔ Delete goal
    /// ✔ Add milestone
    /// ✔ Update milestone progress
    /// ✔ Fetch user goals
    ///
    /// Architecture:
    /// -------------
    /// API Layer → Application Layer → Domain → Infrastructure
    /// </summary>
    public class GoalsController : BaseApiController
    {
        private readonly IGoalService _goalService;

        /// <summary>
        /// Constructor Injection
        /// </summary>
        public GoalsController(IGoalService goalService)
        {
            _goalService = goalService;
        }

        // -------------------------------------------------------
        // CREATE GOAL
        // -------------------------------------------------------

        /// <summary>
        /// Creates a new goal.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateGoal([FromBody] CreateGoalRequest request)
        {
            var goalId = await _goalService.CreateGoalAsync(request);

            return ApiCreated(new
            {
                GoalId = goalId
            });
        }

        // -------------------------------------------------------
        // UPDATE GOAL
        // -------------------------------------------------------

        /// <summary>
        /// Updates existing goal.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGoal(Guid id, [FromBody] UpdateGoalRequest request)
        {
            await _goalService.UpdateGoalAsync(id, request);

            return ApiOk();
        }

        // -------------------------------------------------------
        // DELETE GOAL
        // -------------------------------------------------------

        /// <summary>
        /// Soft deletes a goal.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal(Guid id)
        {
            await _goalService.DeleteGoalAsync(id);

            return ApiOk();
        }

        // -------------------------------------------------------
        // MILESTONE
        // -------------------------------------------------------

        /// <summary>
        /// Adds milestone to goal.
        /// </summary>
        [HttpPost("milestone")]
        public async Task<IActionResult> AddMilestone([FromBody] AddMilestoneRequest request)
        {
            await _goalService.AddMilestoneAsync(request);

            return ApiOk();
        }

        /// <summary>
        /// Updates milestone progress.
        /// </summary>
        [HttpPut("milestone")]
        public async Task<IActionResult> UpdateMilestoneProgress([FromBody] UpdateMilestoneProgressRequest request)
        {
            await _goalService.UpdateMilestoneProgressAsync(request);

            return ApiOk();
        }

        // -------------------------------------------------------
        // QUERY
        // -------------------------------------------------------

        /// <summary>
        /// Returns all goals for a user.
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserGoals(Guid userId)
        {
            var result = await _goalService.GetUserGoalsAsync(userId);

            return ApiOk(result);
        }
    }
}
