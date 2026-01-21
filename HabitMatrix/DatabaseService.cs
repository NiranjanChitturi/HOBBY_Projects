
using Microsoft.Data.SqlClient;
using System.Data;

public class DatabaseService(string connectionString)
{
    public async Task<DataTable> ExecuteQueryAsync(string sql, params SqlParameter[] parameters)
    {
        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddRange(parameters);
        
        var table = new DataTable();
        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        table.Load(reader);
        return table;
    }

    public async Task<int> ExecuteNonQueryAsync(string sql, params SqlParameter[] parameters)
    {
        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddRange(parameters);
        await connection.OpenAsync();
        return await command.ExecuteNonQueryAsync();
    }
}
