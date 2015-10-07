namespace AppFramework.DataProxy.Providers
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.EntityClient;
    using System.Data.Objects;
    using System.Linq;

    /// <summary>
    /// Provides the data-related operations using EntityFramework ORM
    /// </summary>
    internal class EntityDataProvider : IDataProvider
    {
        public IDbConnection DbConnection
        {
            get { return _context.Connection; }
        }

        public string ConnectionString
        {
            get
            {
                return _context.Connection.ConnectionString;
            }
        }

        private readonly ObjectContext _context;
        public EntityDataProvider(ObjectContext context)
        {
            _context = context;
        }

        public IEnumerable<T> Query<T>(string commandText, ObjectParameter[] @params, CommandType commandType = CommandType.Text)
        {
            if (commandType == CommandType.StoredProcedure)
            {
                var fetchedData = _context.ExecuteFunction<T>(commandText, @params);
                foreach (var item in fetchedData)
                    yield return item;
            }
            else if (commandType == CommandType.Text)
            {
                var fetchedData = _context.ExecuteStoreQuery<T>(commandText, (from p in @params
                                                                              select new System.Data.SqlClient.SqlParameter(p.Name, p.Value)).ToArray());
                foreach (var item in fetchedData)
                    yield return item;
            }
            else
            {
                throw new System.NotImplementedException();
            }
        }

        public int ExecuteNonQuery(string commandText, IDataParameter[] parameters = null, CommandType commandType = CommandType.Text, bool closeConnection = true)
        {
            return _context.ExecuteStoreCommand(commandText, parameters);
        }

        public object ExecuteScalar(string commandText, IDataParameter[] parameters = null, CommandType commandType = CommandType.Text, bool closeConnection = true)
        {
            EntityCommand command = CreateStoreCommand(_context, commandText, parameters, commandType);
            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();
            var result = command.ExecuteScalar();
            if (closeConnection)
                command.Connection.Close();
            return result;
        }

        public IDataReader ExecuteReader(string commandText, IDataParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            EntityCommand command = CreateStoreCommand(_context, commandText, parameters, commandType);
            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        private static EntityCommand CreateStoreCommand(ObjectContext context, string commandText, IDataParameter[] parameters, CommandType commandType)
        {
            EntityConnection entityConnection = (EntityConnection)context.Connection;
            EntityCommand entityCommand = entityConnection.CreateCommand();
            entityCommand.CommandType = commandType;
            entityCommand.CommandText = commandText;
            if (null != parameters && parameters.Length > 0)
            {
                entityCommand.Parameters.AddRange(parameters);
            }
            return entityCommand;
        }
    }
}
