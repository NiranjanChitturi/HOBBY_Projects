using MPHMS.Domain.Common;
using System;

namespace MPHMS.Domain.Entities.Goals
{
    /// <summary>
    /// Milestone represents progress checkpoints inside a goal.
    ///
    /// Example:
    /// --------
    /// Goal: Lose 10kg
    /// Milestone: Lose first 3kg
    /// </summary>
    public class Milestone : BaseAuditableEntity
    {
        /// <summary>
        /// Parent goal reference
        /// </summary>
        public Guid GoalId { get; set; }

        /// <summary>
        /// Milestone title
        /// </summary>
        public string Title { get; set; } = null!;

        /// <summary>
        /// Target value required to complete milestone
        /// </summary>
        public decimal TargetValue { get; set; }

        /// <summary>
        /// Current achieved value
        /// </summary>
        public decimal CurrentValue { get; set; }

        // Navigation
        public Goal Goal { get; set; } = null!;
    }
}
