namespace MPHMS.Application.DTOs.Goals
{
    /// <summary>
    /// DTO for updating goals.
    /// </summary>
    public class UpdateGoalRequest
    {
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsCompleted { get; set; }
    }
}
