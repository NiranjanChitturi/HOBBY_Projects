namespace MPHMS.Application.DTOs.Habits
{
    /// <summary>
    /// Response DTO returned to API/UI layer
    /// when fetching habit information.
    ///
    /// This is a READ MODEL.
    /// It is optimized for display purposes
    /// and should NOT contain domain logic.
    /// </summary>
    public class HabitResponse
    {
        /// <summary>
        /// Primary identifier of habit
        /// </summary>
        public Guid HabitId { get; set; }

        /// <summary>
        /// Display name of habit
        /// Example: Daily Workout
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Difficulty lookup reference
        /// Example: Easy = 1, Medium = 2, Hard = 3
        /// </summary>
        public int Difficulty { get; set; }

        /// <summary>
        /// Status lookup reference
        /// Example: Active = 1, Paused = 2, Archived = 3
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Optional category mapping
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Indicates if habit is active
        /// Derived from Status
        /// </summary>
        public bool IsActive => Status == 1;
    }
}
