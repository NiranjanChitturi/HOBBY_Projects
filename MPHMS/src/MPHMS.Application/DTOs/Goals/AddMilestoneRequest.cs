using System;

namespace MPHMS.Application.DTOs.Goals
{
    /// <summary>
    /// DTO used to add a milestone to a goal.
    ///
    /// Example:
    /// --------
    /// Goal: Lose 10kg
    /// Milestone: Lose first 3kg
    /// </summary>
    public class AddMilestoneRequest
    {
        /// <summary>
        /// Parent goal identifier
        /// </summary>
        public Guid GoalId { get; set; }

        /// <summary>
        /// Milestone title
        /// Example: "Lose first 3kg"
        /// </summary>
        public string Title { get; set; } = null!;

        /// <summary>
        /// Target value to reach milestone completion
        /// Example: 3
        /// </summary>
        public decimal TargetValue { get; set; }
    }
}
