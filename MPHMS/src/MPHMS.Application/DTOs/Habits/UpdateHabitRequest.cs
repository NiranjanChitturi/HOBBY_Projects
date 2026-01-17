using System;

namespace MPHMS.Application.DTOs.Habits
{
    /// <summary>
    /// DTO used for updating habit details.
    ///
    /// IMPORTANT:
    /// ----------
    /// - HabitId comes from API route parameter
    /// - Only mutable fields are included
    /// - Must stay aligned with Domain Habit entity
    /// </summary>
    public class UpdateHabitRequest
    {
        /// <summary>
        /// Updated habit name.
        /// Example: "Evening Walk"
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Difficulty lookup reference.
        /// Example:
        /// 1 = Easy
        /// 2 = Medium
        /// 3 = Hard
        /// </summary>
        public int Difficulty { get; set; }

        /// <summary>
        /// Optional category reference.
        /// Maps to HabitCategory table.
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Habit lifecycle status.
        ///
        /// Example:
        /// --------
        /// 1 = Active
        /// 2 = Paused
        /// 3 = Archived
        /// </summary>
        public int Status { get; set; }
    }
}
