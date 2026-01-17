using System;

namespace MPHMS.Application.DTOs.Goals
{
    /// <summary>
    /// DTO used for creating a new goal.
    ///
    /// IMPORTANT:
    /// ----------
    /// UserId is derived from JWT token.
    /// Client MUST NOT supply UserId.
    /// </summary>
    public class CreateGoalRequest
    {
       // /// <summary>
       // /// Owner of the goal
       // /// </summary>
       //// public Guid UserId { get; set; }

        /// <summary>
        /// Goal title
        /// Example: "Lose 5 Kg"
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Optional description
        /// </summary>
        public string? Description { get; set; }

        ///// <summary>
        ///// Owner of the goal
        ///// </summary>
        //public Guid UserId { get; set; }

        /// <summary>
        /// Optional category reference
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Goal start date
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Goal target completion date
        /// </summary>
        public DateOnly TargetDate { get; set; }
    }
}
