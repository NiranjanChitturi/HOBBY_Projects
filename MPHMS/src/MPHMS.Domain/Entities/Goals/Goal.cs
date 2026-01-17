using MPHMS.Domain.Common;
using System;
using System.Collections.Generic;

namespace MPHMS.Domain.Entities.Goals
{
    /// <summary>
    /// Goal represents a long-term objective tracked by the user.
    ///
    /// Examples:
    /// ---------
    /// - Lose 10kg
    /// - Save ₹1,00,000
    /// - Complete AWS Certification
    ///
    /// This is a CORE aggregate root for goal tracking.
    /// </summary>
    public class Goal : BaseAuditableEntity
    {
        /// <summary>
        /// Owner of the goal
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Goal title
        /// Example: "Lose 10kg"
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Optional goal description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Optional category (Fitness, Finance, Career)
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// When the goal tracking starts
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Target completion date
        /// </summary>
        public DateOnly TargetDate { get; set; }

        /// <summary>
        /// Status lookup reference
        /// Example:
        /// 1 = Active
        /// 2 = Completed
        /// 3 = Cancelled
        /// </summary>
        public int Status { get; set; }

        // -----------------------
        // Navigation Properties
        // -----------------------

        public GoalCategory? Category { get; set; }

        public ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
    }
}
