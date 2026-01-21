using System;

namespace HabitMatrix.Models
{
    /// <summary>
    /// Daily completion record for a habit.
    /// One log per habit per day (enforced by SQL unique constraint).
    /// </summary>
    public class HabitLog
    {
        /// <summary>
        /// Primary key (UNIQUEIDENTIFIER).
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// FK to Habits.Id.
        /// </summary>
        public Guid HabitId { get; set; }

        /// <summary>
        /// Date of the log (no time component).
        /// </summary>
        public DateOnly LogDate { get; set; }

        /// <summary>
        /// Completed (true) or missed (false).
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Optional notes for the day.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Created timestamp (UTC).
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Last modification timestamp (UTC).
        /// </summary>
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// Who modified this record (FK to Users.Id).
        /// </summary>
        public Guid? ModifiedBy { get; set; }

        /// <summary>
        /// Soft delete flag (true = hidden).
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}