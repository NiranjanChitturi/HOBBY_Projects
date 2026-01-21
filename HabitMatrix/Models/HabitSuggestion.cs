using System;

namespace HabitMatrix.Models
{
    /// <summary>
    /// Default habit suggestion linked to a category.
    /// </summary>
    public class HabitSuggestion
    {
        public Guid Id { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsEditable { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}