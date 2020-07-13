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
        Task<int> Insert(string query, params DBParam[] paramArray);
        Task<int> Update(string query, params DBParam[] paramArray);
        Task<bool> Delete(string query, params DBParam[] paramArray);
    }

    public class DbAccess : IDbAccess
    {
        SQLiteConnection _connection;

        public DbAccess(SQLiteConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<List<object[]>>> Select(string query, params DBParam[] paramArray)
        {
            _connection.Open();
            try
            {
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                foreach (DBParam param in paramArray)
                {
                    SQLiteParameter commandParam = new SQLiteParameter
                    {
                        ParameterName = param.Name,
                        Value = param.Value
                    };
                    command.Parameters.Add(commandParam);
                }
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
                            {
                                rowArr[col] = reader.GetValue(col);
                            }

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
            finally
            {
                _connection.Close();
            }
        }

        public async Task<int> Insert(string query, params DBParam[] paramArray)
        {
            _connection.Open();
            try
            {
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                foreach (var param in paramArray)
                {
                    var commandParam = new SQLiteParameter
                    {
                        ParameterName = param.Name,
                        Value = param.Value
                    };
                    command.Parameters.Add(commandParam);
                }
                await command.PrepareAsync();
                return await command.ExecuteNonQueryAsync();
            }
            finally
            {
                _connection.Close();
            }
        }

        public async Task<int> Update(string query, params DBParam[] paramArray)
        {
            _connection.Open();
            try
            {
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                foreach (var param in paramArray)
                {
                    var commandParam = new SQLiteParameter
                    {
                        ParameterName = param.Name,
                        Value = param.Value
                    };
                    command.Parameters.Add(commandParam);
                }
                await command.PrepareAsync();
                return await command.ExecuteNonQueryAsync();
            }
            finally
            {
                _connection.Close();
            }
        }

        public async Task<bool> Delete(string query, params DBParam[] paramArray)
        {
            _connection.Open();
            try
            {
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                foreach (var param in paramArray)
                {
                    var commandParam = new SQLiteParameter
                    {
                        ParameterName = param.Name,
                        Value = param.Value
                    };
                    command.Parameters.Add(commandParam);
                }
                await command.PrepareAsync();
                return await command.ExecuteNonQueryAsync() > 0;
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
