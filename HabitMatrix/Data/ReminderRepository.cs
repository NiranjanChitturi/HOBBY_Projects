using HabitMatrix.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace HabitMatrix.Data
{
    /// <summary>
    /// Repository for accessing and modifying Reminders in the database.
    /// </summary>
    public class ReminderRepository
    {
        private readonly DatabaseService _db;

        public ReminderRepository(DatabaseService db)
        {
            _db = db;
        }

        /// <summary>
        /// Get all reminders for a specific user.
        /// </summary>
        public async Task<List<Reminder>> GetByUserAsync(Guid userId)
        {
            string sql = "SELECT * FROM Reminders WHERE UserId = @UserId AND IsDeleted = 0 ORDER BY ScheduledAt";
            var table = await _db.ExecuteQueryAsync(sql, new SqlParameter("@UserId", userId));

            var reminders = new List<Reminder>();
            foreach (DataRow row in table.Rows)
            {
                reminders.Add(MapRowToReminder(row));
            }
            return reminders;
        }

        /// <summary>
        /// Create a new reminder.
        /// </summary>
        public async Task<int> CreateAsync(Reminder reminder)
        {
            string sql = @"
                INSERT INTO Reminders (Id, UserId, HabitId, Message, ScheduledAt, Sent, CreatedAt, IsDeleted)
                VALUES (@Id, @UserId, @HabitId, @Message, @ScheduledAt, 0, SYSUTCDATETIME(), 0)";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@Id", reminder.Id),
                new SqlParameter("@UserId", reminder.UserId),
                new SqlParameter("@HabitId", (object?)reminder.HabitId ?? DBNull.Value),
                new SqlParameter("@Message", reminder.Message),
                new SqlParameter("@ScheduledAt", reminder.ScheduledAt));
        }

        /// <summary>
        /// Mark reminder as sent.
        /// </summary>
        public async Task<int> MarkSentAsync(Guid id)
        {
            string sql = @"
                UPDATE Reminders
                SET Sent = 1,
                    ModifiedAt = SYSUTCDATETIME()
                WHERE Id = @Id";

            return await _db.ExecuteNonQueryAsync(sql, new SqlParameter("@Id", id));
        }

        /// <summary>
        /// Soft delete a reminder.
        /// </summary>
        public async Task<int> SoftDeleteAsync(Guid id, Guid modifiedBy)
        {
            string sql = @"
                UPDATE Reminders
                SET IsDeleted = 1,
                    ModifiedAt = SYSUTCDATETIME(),
                    ModifiedBy = @ModifiedBy
                WHERE Id = @Id";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@ModifiedBy", modifiedBy),
                new SqlParameter("@Id", id));
        }

        /// <summary>
        /// Helper method: maps a DataRow to a Reminder object.
        /// </summary>
        private Reminder MapRowToReminder(DataRow row)
        {
            return new Reminder
            {
                Id = (Guid)row["Id"],
                UserId = (Guid)row["UserId"],
                HabitId = row["HabitId"] == DBNull.Value ? null : (Guid?)row["HabitId"],
                Message = row["Message"].ToString()!,
                ScheduledAt = (DateTime)row["ScheduledAt"],
                Sent = (bool)row["Sent"],
                CreatedAt = row["CreatedAt"] as DateTime?,
                ModifiedAt = row["ModifiedAt"] as DateTime?,
                ModifiedBy = row["ModifiedBy"] as Guid?,
                IsDeleted = (bool)row["IsDeleted"]
            };
        }
    }
}