namespace AppFramework.DataProxy.Providers
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Objects;

    public interface IDataProvider
    {
        /// <summary>
        /// Executes a query directly against the data source that returns a sequence of typed results. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">The command to execute, in the native language of the data source.</param>
        /// <param name="params">An array of parameters to pass to the command. The parameters value can be an array of DbParameter objects or an array of parameter values.</param>
        /// <returns>An enumeration of objects of type TResult.</returns>
        IEnumerable<T> Query<T>(string commandText, ObjectParameter[] @params, CommandType commandType = CommandType.Text);

        /// <summary>
        /// Executes an arbitrary command directly against the data source using the
        //  existing connection.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns>Number of records affected</returns>
        int ExecuteNonQuery(string commandText, IDataParameter[] parameters = null, CommandType commandType = CommandType.Text, bool closeConnection = true);

        object ExecuteScalar(string commandText, IDataParameter[] parameters = null, CommandType commandType = CommandType.Text, bool closeConnection = true);

        IDataReader ExecuteReader(string commandText, IDataParameter[] parameters = null, CommandType commandType = CommandType.Text);

        string ConnectionString { get; }

        IDbConnection DbConnection { get; }
    }
}
