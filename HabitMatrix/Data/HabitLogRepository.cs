using HabitMatrix.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace HabitMatrix.Data
{
    /// <summary>
    /// Repository for accessing and modifying HabitLogs in the database.
    /// Uses DatabaseService for SQL execution.
    /// </summary>
    public class HabitLogRepository
    {
        private readonly DatabaseService _db;

        /// <summary>
        /// Constructor: inject DatabaseService (registered in Program.cs).
        /// </summary>
        public HabitLogRepository(DatabaseService db)
        {
            _db = db;
        }

        /// <summary>
        /// Get all logs for a specific habit.
        /// </summary>
        public async Task<List<HabitLog>> GetByHabitAsync(Guid habitId)
        {
            string sql = "SELECT * FROM HabitLogs WHERE HabitId = @HabitId AND IsDeleted = 0 ORDER BY LogDate";
            var table = await _db.ExecuteQueryAsync(sql, new SqlParameter("@HabitId", habitId));

            var logs = new List<HabitLog>();
            foreach (DataRow row in table.Rows)
            {
                logs.Add(MapRowToHabitLog(row));
            }
            return logs;
        }

        /// <summary>
        /// Get a single log by Id.
        /// </summary>
        public async Task<HabitLog?> GetByIdAsync(Guid id)
        {
            string sql = "SELECT * FROM HabitLogs WHERE Id = @Id AND IsDeleted = 0";
            var table = await _db.ExecuteQueryAsync(sql, new SqlParameter("@Id", id));

            if (table.Rows.Count == 0) return null;
            return MapRowToHabitLog(table.Rows[0]);
        }

        /// <summary>
        /// Create a new log entry for a habit.
        /// Enforces one log per habit per day (SQL constraint).
        /// </summary>
        public async Task<int> CreateAsync(HabitLog log)
        {
            string sql = @"
                INSERT INTO HabitLogs (Id, HabitId, LogDate, Status, Notes, CreatedAt, IsDeleted)
                VALUES (@Id, @HabitId, @LogDate, @Status, @Notes, SYSUTCDATETIME(), 0)";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@Id", log.Id),
                new SqlParameter("@HabitId", log.HabitId),
                new SqlParameter("@LogDate", log.LogDate.ToDateTime(TimeOnly.MinValue)),
                new SqlParameter("@Status", log.Status),
                new SqlParameter("@Notes", (object?)log.Notes ?? DBNull.Value));
        }

        /// <summary>
        /// Update an existing log (status or notes).
        /// Automatically sets ModifiedAt and ModifiedBy.
        /// </summary>
        public async Task<int> UpdateAsync(HabitLog log, Guid modifiedBy)
        {
            string sql = @"
                UPDATE HabitLogs
                SET Status = @Status,
                    Notes = @Notes,
                    ModifiedAt = SYSUTCDATETIME(),
                    ModifiedBy = @ModifiedBy
                WHERE Id = @Id AND IsDeleted = 0";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@Status", log.Status),
                new SqlParameter("@Notes", (object?)log.Notes ?? DBNull.Value),
                new SqlParameter("@ModifiedBy", modifiedBy),
                new SqlParameter("@Id", log.Id));
        }

        /// <summary>
        /// Soft delete a log (mark IsDeleted = 1).
        /// Automatically sets ModifiedAt and ModifiedBy.
        /// </summary>
        public async Task<int> SoftDeleteAsync(Guid id, Guid modifiedBy)
        {
            string sql = @"
                UPDATE HabitLogs
                SET IsDeleted = 1,
                    ModifiedAt = SYSUTCDATETIME(),
                    ModifiedBy = @ModifiedBy
                WHERE Id = @Id";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@ModifiedBy", modifiedBy),
                new SqlParameter("@Id", id));
        }

        /// <summary>
        /// Helper method: maps a DataRow to a HabitLog object.
        /// </summary>
        private HabitLog MapRowToHabitLog(DataRow row)
        {
            return new HabitLog
            {
                Id = (Guid)row["Id"],
                HabitId = (Guid)row["HabitId"],
                LogDate = DateOnly.FromDateTime((DateTime)row["LogDate"]),
                Status = (bool)row["Status"],
                Notes = row["Notes"] == DBNull.Value ? null : row["Notes"].ToString(),
                CreatedAt = row["CreatedAt"] as DateTime?,
                ModifiedAt = row["ModifiedAt"] as DateTime?,
                ModifiedBy = row["ModifiedBy"] as Guid?,
                IsDeleted = (bool)row["IsDeleted"]
            };
        }


        public async Task<HabitLog?> GetByHabitAndDateAsync(Guid habitId, DateTime logDate)
        {
            string sql = @"SELECT TOP 1 * 
                   FROM HabitLogs 
                   WHERE HabitId = @HabitId 
                     AND LogDate = @LogDate 
                     AND IsDeleted = 0";

            var table = await _db.ExecuteQueryAsync(
                sql,
                new SqlParameter("@HabitId", habitId),
                new SqlParameter("@LogDate", DateOnly.FromDateTime(logDate).ToDateTime(TimeOnly.MinValue))
            );

            if (table.Rows.Count == 0)
                return null;

            var row = table.Rows[0];

            return new HabitLog
            {
                Id = (Guid)row["Id"],
                HabitId = (Guid)row["HabitId"],
                LogDate = DateOnly.FromDateTime((DateTime)row["LogDate"]),
                Status = (bool)row["Status"],
                Notes = row["Notes"] == DBNull.Value ? null : (string)row["Notes"],
                CreatedAt = (DateTime)row["CreatedAt"],
                IsDeleted = (bool)row["IsDeleted"]
            };
        }
        //public async Task<HabitLog?> GetByHabitAndDateAsync(Guid habitId, DateOnly logDate)
        //{
        //    string sql = @"SELECT TOP 1 * 
        //           FROM HabitLogs 
        //           WHERE HabitId = @HabitId 
        //             AND LogDate = @LogDate 
        //             AND IsDeleted = 0";

        //    using var reader = await _db.ExecuteReaderAsync(
        //        sql,
        //        new SqlParameter("@HabitId", habitId),
        //        new SqlParameter("@LogDate", logDate.ToDateTime(TimeOnly.MinValue))
        //    );

        //    if (await reader.ReadAsync())
        //    {
        //        return new HabitLog
        //        {
        //            Id = reader.GetGuid(reader.GetOrdinal("Id")),
        //            HabitId = reader.GetGuid(reader.GetOrdinal("HabitId")),
        //            LogDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("LogDate"))),
        //            Status = reader.GetBoolean(reader.GetOrdinal("Status")),
        //            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
        //            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
        //            IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
        //        };
        //    }

        //    return null;


        //    //using var conn = new SqlConnection(_connectionString);
        //    //await conn.OpenAsync();

        //    //var cmd = new SqlCommand(
        //    //    "SELECT TOP 1 * FROM HabitLogs WHERE HabitId = @HabitId AND LogDate = @LogDate AND IsDeleted = 0",
        //    //    conn);

        //    //cmd.Parameters.AddWithValue("@HabitId", habitId);
        //    //cmd.Parameters.AddWithValue("@LogDate", logDate);

        //    //using var reader = await cmd.ExecuteReaderAsync();
        //    //if (await reader.ReadAsync())
        //    //{
        //    //    return new HabitLog
        //    //    {
        //    //        Id = reader.GetGuid(reader.GetOrdinal("Id")),
        //    //        HabitId = reader.GetGuid(reader.GetOrdinal("HabitId")),
        //    //        LogDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("LogDate"))),
        //    //        Status = reader.GetBoolean(reader.GetOrdinal("Status")),
        //    //        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
        //    //        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
        //    //        IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
        //    //    };
        //    //}

        //    //return null;
        //}

    }
}