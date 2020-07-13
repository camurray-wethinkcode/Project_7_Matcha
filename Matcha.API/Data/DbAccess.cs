using System.Data.SQLite;
using System.Data;
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
        Task<DataTable> Select(string query, params DBParam[] paramArray);
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

        public async Task<DataTable> Select(string query, params DBParam[] paramArray)
        {
            _connection.Open();
            try
            {
                var dataTable = new DataTable();

                SQLiteDataAdapter myDataAdapter = new SQLiteDataAdapter(query, _connection);
                foreach (var param in paramArray)
                {
                    var commandParam = new SQLiteParameter
                    {
                        ParameterName = param.Name,
                        Value = param.Value
                    };
                    myDataAdapter.SelectCommand.Parameters.Add(commandParam);
                }
                myDataAdapter.Fill(dataTable);

                return dataTable;
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
                SQLiteCommand myCommand = new SQLiteCommand(query, _connection);
                foreach (var param in paramArray)
                {
                    var commandParam = new SQLiteParameter
                    {
                        ParameterName = param.Name,
                        Value = param.Value
                    };
                    myCommand.Parameters.Add(commandParam);
                }
                await myCommand.PrepareAsync();
                return await myCommand.ExecuteNonQueryAsync();
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
                SQLiteCommand myCommand = new SQLiteCommand(query, _connection);
                foreach (var param in paramArray)
                {
                    var commandParam = new SQLiteParameter
                    {
                        ParameterName = param.Name,
                        Value = param.Value
                    };
                    myCommand.Parameters.Add(commandParam);
                }
                await myCommand.PrepareAsync();
                return await myCommand.ExecuteNonQueryAsync();
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
                SQLiteCommand myCommand = new SQLiteCommand(query, _connection);
                foreach (var param in paramArray)
                {
                    var commandParam = new SQLiteParameter
                    {
                        ParameterName = param.Name,
                        Value = param.Value
                    };
                    myCommand.Parameters.Add(commandParam);
                }
                await myCommand.PrepareAsync();
                return await myCommand.ExecuteNonQueryAsync() > 0;
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
