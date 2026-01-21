using System;

namespace HabitMatrix.Models
{
    /// <summary>
    /// Badge or milestone earned by a user (e.g., "7-day streak").
    /// </summary>
    public class Achievement
    {
        /// <summary>
        /// Primary key (UNIQUEIDENTIFIER).
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// FK to Users.Id (who earned it).
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Title of the achievement (e.g., "Consistency Star").
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Optional description for context.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// When the achievement was earned (UTC).
        /// </summary>
        public DateTime? EarnedAt { get; set; }

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