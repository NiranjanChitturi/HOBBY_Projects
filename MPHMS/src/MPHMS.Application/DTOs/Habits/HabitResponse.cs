using System;

namespace MPHMS.Application.DTOs.Habits
{
    /// <summary>
    /// DTO returned to API/UI.
    /// Never expose EF entities directly.
    /// </summary>
    public class HabitResponse
    {
        public Guid HabitId { get; set; }

        public string Name { get; set; } = null!;

        public bool IsActive { get; set; }

        public string Category { get; set; } = null!;
    }
}
