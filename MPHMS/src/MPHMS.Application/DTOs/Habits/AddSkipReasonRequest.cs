using System;

namespace MPHMS.Application.DTOs.Habits
{
    /// <summary>
    /// DTO for recording skip reason.
    /// </summary>
    public class AddSkipReasonRequest
    {
        public Guid HabitLogId { get; set; }

        public int ReasonId { get; set; }

        public string? Comment { get; set; }
    }
}
