using MPHMS.Domain.Common;
using System;

namespace MPHMS.Domain.Entities.Goals
{
    /// <summary>
    /// Milestone represents a measurable checkpoint
    /// inside a Goal.
    ///
    /// Examples:
    /// ----------
    /// Goal: "Complete AWS Certification"
    /// Milestones:
    /// - Finish Cloud Practitioner course
    /// - Complete Practice Exams
    /// - Book Exam Slot
    ///
    /// Architectural Role:
    /// -------------------
    /// Child Entity (Owned by Goal)
    ///
    /// Important:
    /// ----------
    /// Milestone lifecycle is controlled ONLY via Goal.
    ///
    /// Database Mapping:
    /// -----------------
    /// dbo.GoalMilestones (will be created via EF)
    /// </summary>
    public class Milestone : BaseAuditableEntity
    {
        // /// <summary>
        // /// Primary Key
        // /// </summary>
        // public Guid MilestoneId { get; set; }

        /// <summary>
        /// Foreign key reference to parent Goal
        /// </summary>
        public Guid GoalId { get; set; }

        /// <summary>
        /// Short title of milestone
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Optional milestone description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Completion flag
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// When milestone was completed
        /// </summary>
        public DateTime? CompletedAt { get; private set; }
        
        // Navigation
        public Goal Goal { get; set; } = null!;


        // -------------------------------
        // Domain Behavior
        // -------------------------------

        /// <summary>
        /// Marks milestone as completed.
        ///
        /// Business Rules:
        /// ----------------
        /// - Cannot complete twice
        /// </summary>
        public void MarkCompleted()
        {
            if (IsCompleted)
                throw new InvalidOperationException("Milestone is already completed.");

            IsCompleted = true;
            CompletedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Reopens a completed milestone.
        ///
        /// Used when user reverts progress.
        /// </summary>
        public void Reopen()
        {
            IsCompleted = false;
            CompletedAt = null;
        }
    }
}
