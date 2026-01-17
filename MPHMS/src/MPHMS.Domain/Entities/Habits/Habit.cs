using MPHMS.Domain.Common;
using System;

namespace MPHMS.Domain.Entities.Habits
{
    /// <summary>
    /// Habit is the CORE business entity of MPHMS.
    ///
    /// Represents a recurring activity tracked by the user.
    ///
    /// Examples:
    /// - Morning Run
    /// - Reading 30 minutes
    /// - Meditation
    ///
    /// This entity maps to:
    /// --------------------
    /// dbo.Habits table
    ///
    /// Architecture Layer:
    /// -------------------
    /// Domain Layer (Pure Business Model)
    ///
    /// IMPORTANT:
    /// ----------
    /// - No EF Core attributes
    /// - No database logic
    /// - No framework dependencies
    /// </summary>
    public class Habit : BaseAuditableEntity
    {
        // /// <summary>
        // /// Primary Key
        // /// Maps to: Habits.HabitId
        // /// </summary>
        // public Guid HabitId { get; set; }

        /// <summary>
        /// Owner of the habit
        /// Maps to: Habits.UserId
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Optional category (Health, Fitness, Study etc.)
        /// Maps to: Habits.CategoryId
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Display name of the habit
        /// Example: "Daily Workout"
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Difficulty level reference
        /// Maps to DifficultyLevels lookup table
        /// </summary>
        public int Difficulty { get; set; }

        /// <summary>
        /// Status reference
        /// Maps to HabitStatusLookup table
        /// </summary>
        public int Status { get; set; }

         // Navigation

        public HabitCategory? Category { get; set; }

        public ICollection<HabitLog> Logs { get; set; } = new List<HabitLog>();
   
    }
}
