using MPHMS.Domain.Common;

namespace MPHMS.Domain.Entities.Habits
{
    /// <summary>
    /// SkipReason represents MASTER lookup data
    /// for why users skip habits.
    ///
    /// Examples:
    /// ---------
    /// - Sick
    /// - Travel
    /// - Busy Schedule
    /// - Low Motivation
    /// - Emergency
    ///
    /// This table is ADMIN CONTROLLED.
    ///
    /// Used for:
    /// ----------
    /// - Behavioral analytics
    /// - Skip pattern grouping
    /// - Productivity loss categorization
    ///
    /// Database Mapping:
    /// -----------------
    /// dbo.SkipReasons
    /// </summary>
    public class SkipReason : BaseAuditableEntity
    {
        // /// <summary>
        // /// Primary Key
        // /// Maps to: SkipReasons.ReasonId
        // /// </summary>
        // public int ReasonId { get; set; }

        /// <summary>
        /// Unique short code
        ///
        /// Example:
        /// SICK
        /// BUSY
        /// TRAVEL
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Human readable description
        ///
        /// Example:
        /// "User was sick and unable to perform habit"
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether system predefined
        /// or admin custom created.
        /// </summary>
        public bool IsSystemDefined { get; set; } = true;
    }
}
