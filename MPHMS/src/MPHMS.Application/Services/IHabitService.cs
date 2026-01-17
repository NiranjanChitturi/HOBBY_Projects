using MPHMS.Application.DTOs.Habits;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MPHMS.Application.Services
{
    /// <summary>
    /// IHabitService defines ALL habit-related business operations.
    ///
    /// This is part of the APPLICATION LAYER.
    ///
    /// Responsibilities:
    /// -----------------
    /// ✔ Habit creation
    /// ✔ Habit updates
    /// ✔ Habit deletion (soft)
    /// ✔ Daily habit logging
    /// ✔ Skip reason tracking
    ///
    /// IMPORTANT:
    /// ----------
    /// This interface does NOT contain database logic.
    /// Only business use-case definitions.
    /// </summary>
    public interface IHabitService
    {
        /// <summary>
        /// Creates a new habit for a user.
        /// </summary>
        Task<Guid> CreateHabitAsync(CreateHabitRequest request);

        /// <summary>
        /// Updates an existing habit.
        /// </summary>
        Task UpdateHabitAsync(Guid habitId, UpdateHabitRequest request);

        /// <summary>
        /// Soft deletes a habit.
        /// </summary>
        Task DeleteHabitAsync(Guid habitId);

        /// <summary>
        /// Logs daily habit completion.
        /// </summary>
        Task LogHabitAsync(LogHabitRequest request);

        /// <summary>
        /// Records skip reason for a habit log.
        /// </summary>
        Task AddSkipReasonAsync(AddSkipReasonRequest request);

        /// <summary>
        /// Returns all habits for a user.
        /// </summary>
        Task<List<HabitResponse>> GetUserHabitsAsync(Guid userId);
    }
}
