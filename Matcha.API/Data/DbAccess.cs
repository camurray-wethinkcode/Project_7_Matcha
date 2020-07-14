using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Matcha.API.Data
{
    public struct DBParam
    {
        public DBParam(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name;
        public object Value;
    }

    public interface IDbAccess
    {
        Task<List<List<object[]>>> Select(string query, params DBParam[] paramArray);
        Task<object[]> SelectOne(string query, params DBParam[] paramArray);
        Task<int> NonQuery(string query, params DBParam[] paramArray);
    }

    public class DbAccess : IDbAccess
    {
        string _connectionString;

        public DbAccess(string connectionString) => _connectionString = connectionString;

        /// <summary>Sends a SELECT query to the database, and returns retrieved data</summary>
        /// <param name="query">Query to send. Use @paramName to reference parameter names</param>
        /// <param name="paramArray">Parameters to bind to query. <see cref="DBParam.Name"/> is used to bind to query string</param>
        /// <returns>
        /// Awaitable List of List of Object array.
        /// First level list is for each result set (multiple statements in query), Second level is for each row of data.
        /// Object array contains values retrieved, in order of SELECT fields requested.
        /// </returns>
        public async Task<List<List<object[]>>> Select(string query, params DBParam[] paramArray)
        {
            using SQLiteConnection connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                foreach (var param in paramArray)
                    command.Parameters.Add(new SQLiteParameter(param.Name, param.Value));
                await command.PrepareAsync();

                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    var resultLst = new List<List<object[]>>();

                    while (reader.HasRows)
                    {
                        var rowLst = new List<object[]>();

                        while (await reader.ReadAsync())
                        {
                            var rowArr = new object[reader.FieldCount];

                            for (int col = 0; col < reader.FieldCount; col++)
                                rowArr[col] = reader.GetValue(col);

                            rowLst.Add(rowArr);
                            // add to list of rows
                        }

                        resultLst.Add(rowLst);
                        // add to list of results
                        await reader.NextResultAsync();
                    }

                    return resultLst;
                }
            }
        }

        /// <summary>Sends a SELECT query to the database, and returns first row received</summary>
        /// <param name="query">Query to send. Use @paramName to reference parameter names</param>
        /// <param name="paramArray">Parameters to bind to query. <see cref="DBParam.Name"/> is used to bind to query string</param>
        /// <returns>
        /// Awaitable Object array of values retrieved, in order of SELECT fields requested.
        /// </returns>
        public async Task<object[]> SelectOne(string query, params DBParam[] paramArray)
        {
            var fullResults = await Select(query, paramArray);

            if (fullResults.Count > 0)
            {
                if (fullResults[0].Count > 0)
                    return fullResults[0][0];
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>Sends a query to the database, and returns number of rows affected</summary>
        /// <param name="query">Query to send. Use @paramName to reference parameter names</param>
        /// <param name="paramArray">Parameters to bind to query. <see cref="DBParam.Name"/> is used to bind to query string</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> NonQuery(string query, params DBParam[] paramArray)
        {
            using SQLiteConnection connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                foreach (var param in paramArray)
                    command.Parameters.Add(new SQLiteParameter(param.Name, param.Value));
                await command.PrepareAsync();
                return await command.ExecuteNonQueryAsync();
            }
        }
    }
}
