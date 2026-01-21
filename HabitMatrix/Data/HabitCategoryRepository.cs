using HabitMatrix.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace HabitMatrix.Data
{
    /// <summary>
    /// Repository for accessing HabitCategories in the database.
    /// </summary>
    public class HabitCategoryRepository
    {
        private readonly DatabaseService _db;

        public HabitCategoryRepository(DatabaseService db)
        {
            _db = db;
        }

        /// <summary>
        /// Get all active categories.
        /// </summary>
        public async Task<List<HabitCategory>> GetAllAsync()
        {
            string sql = "SELECT * FROM HabitCategories WHERE IsDeleted = 0";
            var table = await _db.ExecuteQueryAsync(sql);

            var categories = new List<HabitCategory>();
            foreach (DataRow row in table.Rows)
            {
                categories.Add(MapRowToCategory(row));
            }
            return categories;
        }

        /// <summary>
        /// Create a new category (admin only).
        /// </summary>
        public async Task<int> CreateAsync(HabitCategory category)
        {
            string sql = @"
                INSERT INTO HabitCategories (Id, Name, CreatedAt, IsDeleted)
                VALUES (@Id, @Name, SYSUTCDATETIME(), 0)";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@Id", category.Id),
                new SqlParameter("@Name", category.Name));
        }

        /// <summary>
        /// Soft delete a category.
        /// </summary>
        public async Task<int> SoftDeleteAsync(int id, Guid modifiedBy)
        {
            string sql = @"
                UPDATE HabitCategories
                SET IsDeleted = 1,
                    ModifiedAt = SYSUTCDATETIME(),
                    ModifiedBy = @ModifiedBy
                WHERE Id = @Id";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@ModifiedBy", modifiedBy),
                new SqlParameter("@Id", id));
        }

        /// <summary>
        /// Helper method: maps a DataRow to a HabitCategory object.
        /// </summary>
        private HabitCategory MapRowToCategory(DataRow row)
        {
            return new HabitCategory
            {
                Id = (int)row["Id"],
                Name = row["Name"].ToString()!,
                CreatedAt = row["CreatedAt"] as DateTime?,
                ModifiedAt = row["ModifiedAt"] as DateTime?,
                ModifiedBy = row["ModifiedBy"] as Guid?,
                IsDeleted = (bool)row["IsDeleted"]
            };
        }
    }
}