using MPHMS.Domain.Common;
using System;

namespace MPHMS.Domain.Entities.Habits
{
    /// <summary>
    /// HabitCategory represents a logical grouping
    /// of habits for better organization and analytics.
    ///
    /// Examples:
    /// - Health
    /// - Fitness
    /// - Learning
    /// - Work
    ///
    /// Database Mapping:
    /// -----------------
    /// dbo.HabitCategories
    ///
    /// Architecture:
    /// -------------
    /// Domain Layer (Pure Business Object)
    ///
    /// Notes:
    /// ------
    /// Categories are configurable by user/admin.
    /// Used for reporting and filtering.
    /// </summary>
    public class HabitCategory : BaseAuditableEntity
    {
        /// <summary>
        /// Primary Key
        /// Maps to: HabitCategories.CategoryId
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Display name of the category
        /// Example: "Health"
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Controls UI ordering
        /// Lower number = higher priority
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Indicates whether category is active
        /// Inactive categories are hidden but not deleted
        /// </summary>
        public bool IsActive { get; set; }
    }
}
