using HabitMatrix.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace HabitMatrix.Data
{
    /// <summary>
    /// Repository for accessing and modifying Achievements in the database.
    /// </summary>
    public class AchievementRepository
    {
        private readonly DatabaseService _db;

        public AchievementRepository(DatabaseService db)
        {
            _db = db;
        }

        /// <summary>
        /// Get all achievements for a specific user.
        /// </summary>
        public async Task<List<Achievement>> GetByUserAsync(Guid userId)
        {
            string sql = "SELECT * FROM Achievements WHERE UserId = @UserId AND IsDeleted = 0";
            var table = await _db.ExecuteQueryAsync(sql, new SqlParameter("@UserId", userId));

            var achievements = new List<Achievement>();
            foreach (DataRow row in table.Rows)
            {
                achievements.Add(MapRowToAchievement(row));
            }
            return achievements;
        }

        /// <summary>
        /// Create a new achievement.
        /// </summary>
        public async Task<int> CreateAsync(Achievement achievement)
        {
            string sql = @"
                INSERT INTO Achievements (Id, UserId, Title, Description, EarnedAt, IsDeleted)
                VALUES (@Id, @UserId, @Title, @Description, SYSUTCDATETIME(), 0)";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@Id", achievement.Id),
                new SqlParameter("@UserId", achievement.UserId),
                new SqlParameter("@Title", achievement.Title),
                new SqlParameter("@Description", (object?)achievement.Description ?? DBNull.Value));
        }

        /// <summary>
        /// Soft delete an achievement.
        /// </summary>
        public async Task<int> SoftDeleteAsync(Guid id, Guid modifiedBy)
        {
            string sql = @"
                UPDATE Achievements
                SET IsDeleted = 1,
                    ModifiedAt = SYSUTCDATETIME(),
                    ModifiedBy = @ModifiedBy
                WHERE Id = @Id";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@ModifiedBy", modifiedBy),
                new SqlParameter("@Id", id));
        }

        /// <summary>
        /// Helper method: maps a DataRow to an Achievement object.
        /// </summary>
        private Achievement MapRowToAchievement(DataRow row)
        {
            return new Achievement
            {
                Id = (Guid)row["Id"],
                UserId = (Guid)row["UserId"],
                Title = row["Title"].ToString()!,
                Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                EarnedAt = row["EarnedAt"] as DateTime?,
                ModifiedAt = row["ModifiedAt"] as DateTime?,
                ModifiedBy = row["ModifiedBy"] as Guid?,
                IsDeleted = (bool)row["IsDeleted"]
            };
        }
    }
}