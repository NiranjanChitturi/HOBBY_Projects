using System;

namespace MPHMS.Application.DTOs.Goals
{
    /// <summary>
    /// DTO used to update milestone progress.
    ///
    /// Example:
    /// --------
    /// Milestone: Lose first 3kg
    /// Current progress: 1.5kg
    /// </summary>
    public class UpdateMilestoneProgressRequest
    {
        /// <summary>
        /// Milestone identifier
        /// </summary>
        public Guid MilestoneId { get; set; }

        /// <summary>
        /// Current achieved value
        /// </summary>
        public decimal CurrentValue { get; set; }
    }
}
