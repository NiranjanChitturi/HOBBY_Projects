using System;

namespace MPHMS.Application.DTOs.Habits
{
    /// <summary>
    /// DTO used to create a new habit.
    ///
    /// Represents user input from API/UI layer.
    /// </summary>
    public class CreateHabitRequest
    {
        /// <summary>
        /// DTO used for creating a new habit.
        ///
        /// IMPORTANT:
        /// ----------
        /// UserId is NOT accepted from client.
        /// It will be resolved from JWT via CurrentUserService.
        /// </summary>
        ///// <summary>
        ///// Owner of the habit
        ///// </summary>
        ////public Guid UserId { get; set; }

        /// <summary>
        /// Habit display name
        /// Example: "Morning Workout"
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Difficulty lookup value
        /// Example: 1 = Easy, 2 = Medium, 3 = Hard
        /// </summary>
        public int Difficulty { get; set; }

        /// <summary>
        /// Optional category reference
        /// </summary>
        public Guid? CategoryId { get; set; }
    }
}
