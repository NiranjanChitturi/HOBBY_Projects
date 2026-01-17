using MPHMS.Domain.Common;

namespace MPHMS.Domain.Entities.Goals
{
    /// <summary>
    /// GoalCategory represents high-level goal grouping.
    ///
    /// Examples:
    /// ----------
    /// - Career
    /// - Finance
    /// - Fitness
    /// - Education
    /// - Personal Growth
    ///
    /// Used for:
    /// ----------
    /// - Dashboard grouping
    /// - Analytics segmentation
    /// - Productivity reporting
    ///
    /// Database Mapping:
    /// -----------------
    /// dbo.GoalCategories
    /// </summary>
    public class GoalCategory : BaseAuditableEntity
    {
        // /// <summary>
        // /// Primary Key
        // /// Maps to: GoalCategories.CategoryId
        // /// </summary>
        // public Guid CategoryId { get; set; }

        /// <summary>
        /// Category display name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Controls ordering in UI
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Whether category is active
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
