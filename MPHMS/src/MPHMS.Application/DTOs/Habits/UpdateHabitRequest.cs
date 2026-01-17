namespace MPHMS.Application.DTOs.Habits
{
    /// <summary>
    /// DTO used for updating habit details.
    /// </summary>
    public class UpdateHabitRequest
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int CategoryId { get; set; }

        public bool IsActive { get; set; }
    }
}
