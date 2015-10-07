using System.Data;
using System.Data.SqlClient;

namespace AppFramework.Core.DAL
{
    /// <summary>
    /// Represents User table type
    /// </summary>
    /// CREATE TYPE DynEntityIdsTableType AS TABLE
    //(
    //       id bigint identity(1,1),
    //       DynEntityUid bigint not null,
    //       DynEntityId bigint not null,
    //       DynEntityConfigUid bigint not null
    //)
    public class DynEntityIdsTableType
    {
        public SqlParameter SqlParameter
        {
            get { return _sqlParameter; } 
        }

        private System.Data.SqlClient.SqlParameter _sqlParameter;
        private DataTable _entities;

        public DynEntityIdsTableType(string parameterName)
        {
            _entities = new DataTable();
            //_entities.Columns.Add(new DataColumn()
            //    {
            //        DataType = System.Type.GetType("System.Int64"),
            //        ColumnName = "id",
            //        ReadOnly = true,
            //    });
            _entities.Columns.Add(new DataColumn()
            {
                DataType = System.Type.GetType("System.Int64"),
                ColumnName = "DynEntityUid",
            });
            _entities.Columns.Add(new DataColumn()
            {
                DataType = System.Type.GetType("System.Int64"),
                ColumnName = "DynEntityId",
            });
            _entities.Columns.Add(new DataColumn()
            {
                DataType = System.Type.GetType("System.Int64"),
                ColumnName = "DynEntityConfigUid",
            });

            _sqlParameter = new SqlParameter(parameterName, SqlDbType.Structured);
            _sqlParameter.TypeName = "dbo.DynEntityIdsTableType";
            _sqlParameter.Value = _entities;
        }

        /// <summary>
        /// Adds a new row to the Table Type variable
        /// </summary>
        /// <param name="dynEntityUid"></param>
        /// <param name="dynEntityId"></param>
        /// <param name="dynEntityConfigUid"></param>
        public void AddEntity(long dynEntityUid, long dynEntityId, long dynEntityConfigUid)
        {
            var row = _entities.NewRow();
            row["DynEntityUid"] = dynEntityUid;
            row["DynEntityId"] = dynEntityId;
            row["DynEntityConfigUid"] = dynEntityConfigUid;
            _entities.Rows.Add(row);
        }
    }
}
