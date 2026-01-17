using System;

namespace MPHMS.Application.DTOs.Habits
{
    /// <summary>
    /// DTO for daily habit logging.
    /// </summary>
    public class LogHabitRequest
    {
        public Guid HabitId { get; set; }

        public DateTime LogDate { get; set; }

        public bool IsCompleted { get; set; }
    }
}
