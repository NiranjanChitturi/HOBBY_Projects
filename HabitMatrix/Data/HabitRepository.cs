using HabitMatrix.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace HabitMatrix.Data
{
    /// <summary>
    /// Repository for accessing and modifying Habits in the database.
    /// Uses DatabaseService for SQL execution.
    /// </summary>
    public class HabitRepository
    {
        private readonly DatabaseService _db;

        /// <summary>
        /// Constructor: inject DatabaseService (registered in Program.cs).
        /// </summary>
        public HabitRepository(DatabaseService db)
        {
            _db = db;
        }

        /// <summary>
        /// Get all active habits for a specific user.
        /// </summary>
        public async Task<List<Habit>> GetByUserAsync(Guid userId)
        {
            string sql = "SELECT * FROM Habits WHERE UserId = @UserId AND IsDeleted = 0";
            var table = await _db.ExecuteQueryAsync(sql, new SqlParameter("@UserId", userId));

            var habits = new List<Habit>();
            foreach (DataRow row in table.Rows)
            {
                habits.Add(MapRowToHabit(row));
            }
            return habits;
        }

        /// <summary>
        /// Get a single habit by Id.
        /// </summary>
        public async Task<Habit?> GetByIdAsync(Guid id)
        {
            string sql = "SELECT * FROM Habits WHERE Id = @Id AND IsDeleted = 0";
            var table = await _db.ExecuteQueryAsync(sql, new SqlParameter("@Id", id));

            if (table.Rows.Count == 0) return null;
            return MapRowToHabit(table.Rows[0]);
        }

        /// <summary>
        /// Create a new habit.
        /// </summary>
        public async Task<int> CreateAsync(Habit habit)
        {
            string sql = @"
                INSERT INTO Habits (Id, UserId, Name, Category, CategoryId, Color, CreatedAt, IsDeleted)
                VALUES (@Id, @UserId, @Name, @Category, @CategoryId, @Color, SYSUTCDATETIME(), 0)";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@Id", habit.Id),
                new SqlParameter("@UserId", habit.UserId),
                new SqlParameter("@Name", habit.Name),
                new SqlParameter("@Category", habit.Category),
                new SqlParameter("@CategoryId", (object?)habit.CategoryId ?? DBNull.Value),
                new SqlParameter("@Color", habit.Color),
                new SqlParameter("@CreatedAt", habit.CreatedAt),
                new SqlParameter("@IsDeleted", habit.IsDeleted));
        }

        /// <summary>
        /// Update habit details (name, category, color).
        /// Automatically sets ModifiedAt and ModifiedBy.
        /// </summary>
        public async Task<int> UpdateAsync(Habit habit, Guid modifiedBy)
        {
            string sql = @"
                UPDATE Habits
                SET Name = @Name,
                    Category = @Category,
                    CategoryId = @CategoryId,
                    Color = @Color,
                    ModifiedAt = SYSUTCDATETIME(),
                    ModifiedBy = @ModifiedBy
                WHERE Id = @Id AND IsDeleted = 0";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@Name", habit.Name),
                new SqlParameter("@Category", habit.Category),
                new SqlParameter("@CategoryId", (object?)habit.CategoryId ?? DBNull.Value),
                new SqlParameter("@Color", habit.Color),
                new SqlParameter("@ModifiedBy", modifiedBy),
                new SqlParameter("@Id", habit.Id));
        }

        /// <summary>
        /// Soft delete a habit (mark IsDeleted = 1).
        /// Automatically sets ModifiedAt and ModifiedBy.
        /// </summary>
        public async Task<int> SoftDeleteAsync(Guid id, Guid modifiedBy)
        {
            string sql = @"
                UPDATE Habits
                SET IsDeleted = 1,
                    ModifiedAt = SYSUTCDATETIME(),
                    ModifiedBy = @ModifiedBy
                WHERE Id = @Id";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@ModifiedBy", modifiedBy),
                new SqlParameter("@Id", id));
        }

        /// <summary>
        /// Helper method: maps a DataRow to a Habit object.
        /// </summary>
        private Habit MapRowToHabit(DataRow row)
        {
            return new Habit
            {
                Id = (Guid)row["Id"],
                UserId = (Guid)row["UserId"],
                Name = row["Name"].ToString()!,
                Category = row["Category"].ToString()!,
                CategoryId = row["CategoryId"] == DBNull.Value ? null : (int?)row["CategoryId"],
                Color = row["Color"].ToString()!,
                CreatedAt = row["CreatedAt"] as DateTime?,
                ModifiedAt = row["ModifiedAt"] as DateTime?,
                ModifiedBy = row["ModifiedBy"] as Guid?,
                IsDeleted = (bool)row["IsDeleted"]
            };
        }
    }
}