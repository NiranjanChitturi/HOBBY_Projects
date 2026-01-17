using MPHMS.Domain.Common;
using System;

namespace MPHMS.Domain.Entities.Habits
{
    /// <summary>
    /// HabitSkipLog stores the REASON behind
    /// a skipped habit.
    ///
    /// Example:
    /// --------
    /// Habit: Workout
    /// Date: 2026-01-18
    /// Reason: Feeling Sick
    ///
    /// This enables:
    /// ----------------
    /// - Behavioral analysis
    /// - Skip trend reporting
    /// - Pattern detection
    /// - Root cause productivity loss analysis
    ///
    /// Database Mapping:
    /// -----------------
    /// dbo.HabitSkipLogs
    ///
    /// Relationship:
    /// -------------
    /// ONE HabitLog -> ZERO or ONE HabitSkipLog
    /// ONE SkipReason -> MANY HabitSkipLogs
    /// </summary>
    public class HabitSkipLog : BaseAuditableEntity
    {
        // ----------------------------------------
        // Foreign Keys
        // ----------------------------------------

        /// <summary>
        /// Foreign Key to HabitLog
        /// Links skip reason to specific day
        /// </summary>
        public Guid HabitLogId { get; set; }

        /// <summary>
        /// Foreign Key to SkipReason lookup
        /// </summary>
        public int ReasonId { get; set; }

        // ----------------------------------------
        // Business Data
        // ----------------------------------------

        /// <summary>
        /// Optional user comment
        ///
        /// Example:
        /// "Had fever and doctor advised rest"
        /// </summary>
        public string? Comment { get; set; }

        // ----------------------------------------
        // Navigation Properties
        // ----------------------------------------

        /// <summary>
        /// Reference to HabitLog
        /// </summary>
        public HabitLog HabitLog { get; set; } = null!;

        /// <summary>
        /// Reference to SkipReason lookup
        /// </summary>
        public SkipReason SkipReason { get; set; } = null!;
    }
}
