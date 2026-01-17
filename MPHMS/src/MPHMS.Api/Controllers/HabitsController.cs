using Microsoft.AspNetCore.Mvc;
using MPHMS.Api.Controllers.Base;
using MPHMS.Application.DTOs.Habits;
using MPHMS.Application.Services;
using System;
using System.Threading.Tasks;

namespace MPHMS.Api.Controllers
{
    /// <summary>
    /// HabitsController exposes REST endpoints
    /// for Habit management.
    ///
    /// Responsibilities:
    /// -----------------
    /// ✔ Create habit
    /// ✔ Update habit
    /// ✔ Delete habit
    /// ✔ Log daily habit
    /// ✔ Add skip reason
    /// ✔ Fetch user habits
    ///
    /// Architecture:
    /// -------------
    /// API Layer → Application Layer → Domain → Infrastructure
    /// </summary>
    public class HabitsController : BaseApiController
    {
        private readonly IHabitService _habitService;

        /// <summary>
        /// Constructor Injection
        /// </summary>
        public HabitsController(IHabitService habitService)
        {
            _habitService = habitService;
        }

        // -------------------------------------------------------
        // CREATE HABIT
        // -------------------------------------------------------

        /// <summary>
        /// Creates a new habit.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateHabit([FromBody] CreateHabitRequest request)
        {
            var habitId = await _habitService.CreateHabitAsync(request);

            return ApiCreated(new
            {
                HabitId = habitId
            });
        }

        // -------------------------------------------------------
        // UPDATE HABIT
        // -------------------------------------------------------

        /// <summary>
        /// Updates existing habit.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHabit(Guid id, [FromBody] UpdateHabitRequest request)
        {
            await _habitService.UpdateHabitAsync(id, request);

            return ApiOk();
        }

        // -------------------------------------------------------
        // DELETE HABIT
        // -------------------------------------------------------

        /// <summary>
        /// Soft deletes a habit.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHabit(Guid id)
        {
            await _habitService.DeleteHabitAsync(id);

            return ApiOk();
        }

        // -------------------------------------------------------
        // DAILY LOGGING
        // -------------------------------------------------------

        /// <summary>
        /// Logs daily habit completion.
        /// </summary>
        [HttpPost("log")]
        public async Task<IActionResult> LogHabit([FromBody] LogHabitRequest request)
        {
            await _habitService.LogHabitAsync(request);

            return ApiOk();
        }

        // -------------------------------------------------------
        // SKIP REASON
        // -------------------------------------------------------

        /// <summary>
        /// Adds skip reason for habit log.
        /// </summary>
        [HttpPost("skip")]
        public async Task<IActionResult> AddSkipReason([FromBody] AddSkipReasonRequest request)
        {
            await _habitService.AddSkipReasonAsync(request);

            return ApiOk();
        }

        // -------------------------------------------------------
        // QUERY
        // -------------------------------------------------------

        /// <summary>
        /// Returns all habits for a user.
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserHabits(Guid userId)
        {
            var result = await _habitService.GetUserHabitsAsync(userId);

            return ApiOk(result);
        }
    }
}
