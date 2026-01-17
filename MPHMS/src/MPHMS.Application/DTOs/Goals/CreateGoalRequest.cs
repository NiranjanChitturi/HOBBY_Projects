using System;

namespace MPHMS.Application.DTOs.Goals
{
    /// <summary>
    /// DTO for goal creation.
    /// </summary>
    public class CreateGoalRequest
    {
        public Guid UserId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public int CategoryId { get; set; }

        public DateTime TargetDate { get; set; }
    }
}
