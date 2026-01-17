using System;

namespace MPHMS.Application.DTOs.Goals
{
    /// <summary>
    /// Goal response DTO for API/UI.
    /// </summary>
    public class GoalResponse
    {
        public Guid GoalId { get; set; }

        public string Title { get; set; } = null!;

        public bool IsCompleted { get; set; }

        public string Category { get; set; } = null!;
    }
}
