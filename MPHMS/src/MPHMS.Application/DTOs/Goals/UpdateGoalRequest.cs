using System;

namespace MPHMS.Application.DTOs.Goals
{
    /// <summary>
    /// DTO used for updating Goal details.
    ///
    /// Only modifiable business fields are exposed here.
    /// </summary>
    public class UpdateGoalRequest
    {
        /// <summary>
        /// Updated goal title
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Updated description (optional)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Updated category reference
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Updated target completion date
        /// </summary>
        public DateOnly TargetDate { get; set; }

        /// <summary>
        /// Goal status lookup value
        /// Example:
        /// 1 = Active
        /// 2 = Completed
        /// 3 = Archived
        /// </summary>
        public int Status { get; set; }
    }
}
