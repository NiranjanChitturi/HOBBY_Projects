using System;

namespace HabitMatrix.Models
{
    /// <summary>
    /// Normalized category for habits (e.g., Health, Work).
    /// Optional but useful for clean reporting and suggestions.
    /// </summary>
    public class HabitCategory
    {
        /// <summary>
        /// Identity PK in SQL (INT IDENTITY).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique category name (e.g., "Health").
        /// </summary>
        public string Name { get; set; } = string.Empty;

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