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
    /// ONE HabitLog -> ZERO or ONE SkipReason
    /// </summary>
    public class HabitSkipLog : BaseAuditableEntity
    {
        /// <summary>
        /// Primary Key
        /// Maps to: HabitSkipLogs.SkipLogId
        /// </summary>
        public Guid SkipLogId { get; set; }

        /// <summary>
        /// Foreign Key to HabitLog
        /// Links skip reason to specific day
        /// </summary>
        public Guid HabitLogId { get; set; }

        /// <summary>
        /// Foreign Key to SkipReason lookup
        /// </summary>
        public int ReasonId { get; set; }

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
    }
}
