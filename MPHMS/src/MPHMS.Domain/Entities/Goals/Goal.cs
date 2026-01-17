using MPHMS.Domain.Common;
using System;
using System.Collections.Generic;

namespace MPHMS.Domain.Entities.Goals
{
    /// <summary>
    /// Goal represents a long-term objective defined by a user.
    ///
    /// Examples:
    /// ----------
    /// - Save ₹5 Lakhs by Dec
    /// - Complete AWS Certification
    /// - Lose 10kg in 6 months
    ///
    /// Architectural Role:
    /// -------------------
    /// Aggregate Root
    ///
    /// Owns:
    /// -----
    /// - Milestones
    ///
    /// All changes to Milestones MUST go through Goal.
    ///
    /// Database Mapping:
    /// -----------------
    /// dbo.Goals
    /// </summary>
    public class Goal : BaseAuditableEntity
    {
        /// <summary>
        /// Primary Key
        /// Maps to: Goals.GoalId
        /// </summary>
        public Guid GoalId { get; set; }

        /// <summary>
        /// Foreign key reference to UserProfile
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Optional category grouping
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Goal title displayed to user
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the goal
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Priority scale:
        /// 1 = Low
        /// 5 = Critical
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Optional deadline for completion
        /// </summary>
        public DateTime? Deadline { get; set; }

        /// <summary>
        /// Status reference:
        /// 1 = Active
        /// 2 = Completed
        /// 3 = Archived
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Navigation collection of milestones
        /// </summary>
        public ICollection<Milestone> Milestones { get; set; }
            = new List<Milestone>();

        // -------------------------------
        // Domain Behavior (Business Rules)
        // -------------------------------

        /// <summary>
        /// Marks goal as completed.
        ///
        /// Business Rule:
        /// --------------
        /// - All milestones must be completed
        /// </summary>
        public void MarkCompleted()
        {
            if (Milestones.Any(m => !m.IsCompleted))
                throw new InvalidOperationException(
                    "Cannot complete goal until all milestones are completed.");

            Status = 2; // Completed
        }

        /// <summary>
        /// Archives goal (soft business state).
        ///
        /// Used when user no longer wants to track it.
        /// </summary>
        public void Archive()
        {
            Status = 3; // Archived
        }
    }
}
