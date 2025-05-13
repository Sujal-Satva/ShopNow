using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DataAccess.Helper
{
    public static class AdoHelper
    {
        public static async Task<List<T>> ExecuteReaderListAsync<T>(
            SqlConnection connection,
            string query,
            List<SqlParameter> parameters) where T : new()
        {
            var result = new List<T>();

            using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            command.Parameters.AddRange(parameters.ToArray());

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            var properties = typeof(T).GetProperties();

            while (await reader.ReadAsync())
            {
                var obj = new T();

                foreach (var property in properties)
                {
                    var value = reader[property.Name];
                    if (value != DBNull.Value)
                        property.SetValue(obj, value);
                }

                result.Add(obj);
            }

            return result;
        }


        public static async Task<int> ExecuteNonQueryAsync(
            SqlConnection connection,
            string query,
            List<SqlParameter> parameters,
            SqlTransaction? transaction = null)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            command.Transaction = transaction; // Attach transaction if provided
            command.Parameters.AddRange(parameters.ToArray());

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync();
        }


        public static async Task<object> ExecuteScalarAsync(
            SqlConnection connection,
            string query,
            List<SqlParameter> parameters,
            SqlTransaction? transaction = null)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            command.Transaction = transaction; // Support transaction
            command.Parameters.AddRange(parameters.ToArray());

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            return await command.ExecuteScalarAsync();
        }

    }
}
