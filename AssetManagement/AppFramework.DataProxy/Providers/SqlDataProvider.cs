namespace AppFramework.DataProxy.Providers
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Objects;
    using System.Data.SqlClient;

    /// <summary>
    /// Provides the data-related operations using plain SQL
    /// </summary>
    internal class SqlDataProvider : IDataProvider
    {
        public IDbConnection DbConnection
        {
            get { return _connection; }
        }

        public string ConnectionString
        {
            get
            {
                return _connection.ConnectionString;
            }
        }

        private readonly SqlConnection _connection;
        public SqlDataProvider(SqlConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<T> Query<T>(string commandText, ObjectParameter[] @params, CommandType commandType = CommandType.Text)
        {
            throw new System.NotImplementedException();
        }

        public int ExecuteNonQuery(string commandText, IDataParameter[] parameters = null, CommandType commandType = CommandType.Text, bool closeConnection = true)
        {
            var command = CreateStoreCommand(_connection, commandText, parameters, commandType);
            command.CommandTimeout = 60*60; // 1 hour
            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            var recordsAffected = command.ExecuteNonQuery();
            if (closeConnection)
                command.Connection.Close();
            return recordsAffected;
        }

        public object ExecuteScalar(string commandText, IDataParameter[] parameters = null, CommandType commandType = CommandType.Text, bool closeConnection = true)
        {
            var command = CreateStoreCommand(_connection, commandText, parameters, commandType);
            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();
            var result = command.ExecuteScalar();
            if (closeConnection)
                command.Connection.Close();
            return result;
        }

        public IDataReader ExecuteReader(string commandText, IDataParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            var command = CreateStoreCommand(_connection, commandText, parameters, commandType);
            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        private static IDbCommand CreateStoreCommand(SqlConnection connection, string commandText, IDataParameter[] parameters, CommandType commandType)
        {
            var command = new SqlCommand(commandText, connection) { CommandType = commandType };
            if (null != parameters && parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            command.CommandTimeout = DataProxySettings.CommandTimeout;

            return command;
        }
    }
}
