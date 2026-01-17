using System;

namespace MPHMS.Application.DTOs.Goals
{
    /// <summary>
    /// Response DTO returned to API/UI layer for Goals.
    /// </summary>
    public class GoalResponse
    {
        /// <summary>
        /// Primary Key of Goal
        /// </summary>
        public Guid GoalId { get; set; }

        /// <summary>
        /// Goal title
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Optional description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Current status (lookup reference)
        /// Example: Active, Completed, Paused
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Target completion date
        /// </summary>
        public DateOnly TargetDate { get; set; }
    }
}
