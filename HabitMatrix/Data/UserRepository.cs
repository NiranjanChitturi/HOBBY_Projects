using HabitMatrix.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace HabitMatrix.Data
{
    /// <summary>
    /// Repository for accessing and modifying Users in the database.
    /// Uses DatabaseService for SQL execution.
    /// Responsibilities:
    /// - Query active users
    /// - Fetch by Id, Username, Email
    /// - Create new users (with hashed passwords)
    /// - Update user details
    /// - Soft delete users
    /// </summary>
    public class UserRepository
    {
        private readonly DatabaseService _db;

        /// <summary>
        /// Constructor: inject DatabaseService (registered in Program.cs).
        /// </summary>
        public UserRepository(DatabaseService db)
        {
            _db = db;
        }

        /// <summary>
        /// Get all active (non-deleted) users.
        /// </summary>
        public async Task<List<User>> GetAllAsync()
        {
            string sql = "SELECT * FROM Users WHERE IsDeleted = 0";
            var table = await _db.ExecuteQueryAsync(sql);

            var users = new List<User>();
            foreach (DataRow row in table.Rows)
            {
                users.Add(MapRowToUser(row));
            }
            return users;
        }

        /// <summary>
        /// Get a single user by Id.
        /// </summary>
        public async Task<User?> GetByIdAsync(Guid id)
        {
            string sql = "SELECT * FROM Users WHERE Id = @Id AND IsDeleted = 0";
            var table = await _db.ExecuteQueryAsync(sql, new SqlParameter("@Id", id));

            if (table.Rows.Count == 0) return null;
            return MapRowToUser(table.Rows[0]);
        }

        /// <summary>
        /// Get a single user by Username (case-insensitive).
        /// Useful for login validation.
        /// </summary>
        public async Task<User?> GetByUsernameAsync(string username)
        {
            string sql = "SELECT * FROM Users WHERE LOWER(Username) = LOWER(@Username) AND IsDeleted = 0";
            var table = await _db.ExecuteQueryAsync(sql, new SqlParameter("@Username", username));

            if (table.Rows.Count == 0) return null;
            return MapRowToUser(table.Rows[0]);
        }

        /// <summary>
        /// Get a single user by Email (case-insensitive).
        /// Useful for registration uniqueness check.
        /// </summary>
        public async Task<User?> GetByEmailAsync(string email)
        {
            string sql = "SELECT * FROM Users WHERE LOWER(Email) = LOWER(@Email) AND IsDeleted = 0";
            var table = await _db.ExecuteQueryAsync(sql, new SqlParameter("@Email", email));

            if (table.Rows.Count == 0) return null;
            return MapRowToUser(table.Rows[0]);
        }

        /// <summary>
        /// Create a new user.
        /// PasswordHash should already be hashed before calling this.
        /// </summary>
        public async Task<int> CreateAsync(User user)
        {
            string sql = @"
                INSERT INTO Users (Id, Username, PasswordHash, Email, Role, JoinedAt, IsDeleted)
                VALUES (@Id, @Username, @PasswordHash, @Email, @Role, SYSUTCDATETIME(), 0)";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@Id", user.Id),
                new SqlParameter("@Username", user.Username),
                new SqlParameter("@PasswordHash", user.PasswordHash),
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@Role", user.Role));
        }

        /// <summary>
        /// Alias for CreateAsync to match controller expectations.
        /// Allows AccountController to call AddAsync(newUser).
        /// </summary>
        public async Task<int> AddAsync(User user)
        {
            return await CreateAsync(user);
        }

        /// <summary>
        /// Update user details (email, role, etc.).
        /// Automatically sets ModifiedAt and ModifiedBy.
        /// </summary>
        public async Task<int> UpdateAsync(User user, Guid modifiedBy)
        {
            string sql = @"
                UPDATE Users
                SET Email = @Email,
                    Role = @Role,
                    ModifiedAt = SYSUTCDATETIME(),
                    ModifiedBy = @ModifiedBy
                WHERE Id = @Id AND IsDeleted = 0";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@Role", user.Role),
                new SqlParameter("@ModifiedBy", modifiedBy),
                new SqlParameter("@Id", user.Id));
        }

        /// <summary>
        /// Soft delete a user (mark IsDeleted = 1).
        /// Automatically sets ModifiedAt and ModifiedBy.
        /// </summary>
        public async Task<int> SoftDeleteAsync(Guid id, Guid modifiedBy)
        {
            string sql = @"
                UPDATE Users
                SET IsDeleted = 1,
                    ModifiedAt = SYSUTCDATETIME(),
                    ModifiedBy = @ModifiedBy
                WHERE Id = @Id";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@ModifiedBy", modifiedBy),
                new SqlParameter("@Id", id));
        }

        /// <summary>
        /// Helper method: maps a DataRow to a User object.
        /// Ensures null-safe conversions for optional fields.
        /// </summary>
        private User MapRowToUser(DataRow row)
        {
            return new User
            {
                Id = (Guid)row["Id"],
                Username = row["Username"].ToString()!,
                PasswordHash = row["PasswordHash"].ToString()!,
                Email = row["Email"].ToString()!,
                Role = row["Role"].ToString()!,
                JoinedAt = row["JoinedAt"] as DateTime?,
                ModifiedAt = row["ModifiedAt"] as DateTime?,
                ModifiedBy = row["ModifiedBy"] as Guid?,
                IsDeleted = (bool)row["IsDeleted"],

                // ✅ Add ThemePreference mapping
                ThemePreference = row.Table.Columns.Contains("ThemePreference")
            ? row["ThemePreference"]?.ToString() ?? "light"
            : "light"

            };
        }
    }
}