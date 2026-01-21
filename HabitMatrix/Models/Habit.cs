using System;

namespace HabitMatrix.Models
{
    /// <summary>
    /// A habit owned by a user. Can reference a normalized category.
    /// </summary>
    public class Habit
    {
        /// <summary>
        /// Primary key (UNIQUEIDENTIFIER).
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Owner (FK to Users.Id).
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Display name of the habit (e.g., "Drink Water").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Free-text category (kept for flexibility and UI display).
        /// </summary>
        public string Category { get; set; } = "Health";

        /// <summary>
        /// Optional normalized category (FK to HabitCategories.Id).
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Color used in UI (hex or name, e.g., "#6366f1").
        /// </summary>
        public string Color { get; set; } = "#6366f1";

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