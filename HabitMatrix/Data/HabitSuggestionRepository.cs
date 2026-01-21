using HabitMatrix.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace HabitMatrix.Data
{
    /// <summary>
    /// Repository for accessing and modifying HabitSuggestions in the database.
    /// </summary>
    public class HabitSuggestionRepository
    {
        private readonly DatabaseService _db;

        public HabitSuggestionRepository(DatabaseService db)
        {
            _db = db;
        }

        /// <summary>
        /// Get all suggestions for a category.
        /// </summary>
        public async Task<List<HabitSuggestion>> GetByCategoryAsync(int categoryId)
        {
            string sql = "SELECT * FROM HabitSuggestions WHERE CategoryId = @CategoryId AND IsDeleted = 0";
            var table = await _db.ExecuteQueryAsync(sql, new SqlParameter("@CategoryId", categoryId));

            var suggestions = new List<HabitSuggestion>();
            foreach (DataRow row in table.Rows)
            {
                suggestions.Add(MapRowToSuggestion(row));
            }
            return suggestions;
        }

        /// <summary>
        /// Create a new suggestion (admin only).
        /// </summary>
        public async Task<int> CreateAsync(HabitSuggestion suggestion)
        {
            string sql = @"
                INSERT INTO HabitSuggestions (Id, CategoryId, Title, Description, IsEditable, CreatedAt, IsDeleted)
                VALUES (@Id, @CategoryId, @Title, @Description, @IsEditable, SYSUTCDATETIME(), 0)";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@Id", suggestion.Id),
                new SqlParameter("@CategoryId", suggestion.CategoryId),
                new SqlParameter("@Title", suggestion.Title),
                new SqlParameter("@Description", (object?)suggestion.Description ?? DBNull.Value),
                new SqlParameter("@IsEditable", suggestion.IsEditable));
        }

        /// <summary>
        /// Soft delete a suggestion.
        /// </summary>
        public async Task<int> SoftDeleteAsync(Guid id, Guid modifiedBy)
        {
            string sql = @"
                UPDATE HabitSuggestions
                SET IsDeleted = 1,
                    ModifiedAt = SYSUTCDATETIME(),
                    ModifiedBy = @ModifiedBy
                WHERE Id = @Id";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@ModifiedBy", modifiedBy),
                new SqlParameter("@Id", id));
        }

        /// <summary>
        /// Helper method: maps a DataRow to a HabitSuggestion object.
        /// </summary>
        private HabitSuggestion MapRowToSuggestion(DataRow row)
        {
            return new HabitSuggestion
            {
                Id = (Guid)row["Id"],
                CategoryId = (int)row["CategoryId"],
                Title = row["Title"].ToString()!,
                Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                IsEditable = (bool)row["IsEditable"],
                CreatedAt = row["CreatedAt"] as DateTime?,
                ModifiedAt = row["ModifiedAt"] as DateTime?,
                ModifiedBy = row["ModifiedBy"] as Guid?,
                IsDeleted = (bool)row["IsDeleted"]
            };
        }
    }
}