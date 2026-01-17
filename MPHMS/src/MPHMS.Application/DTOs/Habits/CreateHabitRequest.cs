using System;

namespace MPHMS.Application.DTOs.Habits
{
    /// <summary>
    /// DTO used when creating a new habit.
    ///
    /// Acts as API → Application boundary contract.
    /// </summary>
    public class CreateHabitRequest
    {
        public Guid UserId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int CategoryId { get; set; }

        public bool IsDaily { get; set; }
    }
}
