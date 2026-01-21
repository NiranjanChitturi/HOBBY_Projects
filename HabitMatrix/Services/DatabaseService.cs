using System.Data;
using Microsoft.Data.SqlClient;

namespace HabitMatrix.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Executes a SQL query and returns a DataTable.
        /// </summary>
        public async Task<DataTable> ExecuteQueryAsync(string sql, params SqlParameter[] parameters)
        {
            var table = new DataTable();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    table.Load(reader);
                }
            }

            return table;
        }

        /// <summary>
        /// Executes a SQL command (INSERT/UPDATE/DELETE) and returns affected rows.
        /// </summary>
        public async Task<int> ExecuteNonQueryAsync(string sql, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}