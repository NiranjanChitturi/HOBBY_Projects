using System;

namespace HabitMatrix.Models
{
    /// <summary>
    /// Scheduled notification (email/push) for a user, optionally tied to a habit.
    /// </summary>
    public class Reminder
    {
        /// <summary>
        /// Primary key (UNIQUEIDENTIFIER).
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// FK to Users.Id (recipient).
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Optional FK to Habits.Id (specific habit reminder).
        /// </summary>
        public Guid? HabitId { get; set; }

        /// <summary>
        /// Reminder text (e.g., "Log your workout").
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// When to send the reminder (UTC).
        /// </summary>
        public DateTime ScheduledAt { get; set; }

        /// <summary>
        /// Whether the reminder has been sent.
        /// </summary>
        public bool Sent { get; set; }

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