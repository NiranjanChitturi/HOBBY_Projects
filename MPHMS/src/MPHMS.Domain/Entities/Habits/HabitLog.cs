using MPHMS.Domain.Common;
using System;

namespace MPHMS.Domain.Entities.Habits
{
    /// <summary>
    /// HabitLog represents DAILY execution tracking
    /// of a habit.
    ///
    /// Example:
    /// --------
    /// Habit: Drink Water
    /// Date: 2026-01-18
    /// Status: Completed
    ///
    /// Database Mapping:
    /// -----------------
    /// dbo.HabitLogs
    ///
    /// This table feeds:
    /// -----------------
    /// - Streak calculation
    /// - Productivity analytics
    /// - Behavioral insights
    /// - Habit consistency graphs
    ///
    /// IMPORTANT:
    /// ----------
    /// One habit can have ONLY ONE log per day.
    /// Enforced by unique constraint:
    /// (HabitId + LogDate)
    /// </summary>
    public class HabitLog : BaseAuditableEntity
    {
        // /// <summary>
        // /// Primary Key
        // /// Maps to: HabitLogs.LogId
        // /// </summary>
        // public Guid LogId { get; set; }

        /// <summary>
        /// Foreign Key to Habit
        /// </summary>
        public Guid HabitId { get; set; }

        /// <summary>
        /// Date of execution (no time component)
        /// </summary>
        public DateOnly LogDate { get; set; }

        /// <summary>
        /// Status value
        ///
        /// Maps to:
        /// HabitStatusLookup
        ///
        /// Example values:
        /// 1 = Completed
        /// 2 = Skipped
        /// 3 = Partial
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Optional notes from user
        /// Example:
        /// "Completed late due to meeting"
        /// </summary>
        public string? Notes { get; set; }

        // ----------------------------------------
        // Navigation Properties (EF Core)
        // ----------------------------------------

        /// <summary>
        /// Navigation reference to parent Habit
        /// </summary>
        public Habit Habit { get; set; } = null!;
        
        public HabitSkipLog? SkipLog { get; set; }
    }
}
