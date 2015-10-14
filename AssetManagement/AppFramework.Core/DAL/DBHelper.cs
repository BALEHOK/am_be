using AppFramework.Core.Classes;
using AppFramework.DataProxy;

namespace AppFramework.Core.DAL
{
    using AppFramework.Core.AC.Authentication;
    using AppFramework.Core.ConstantsEnumerators;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public static class DBHelper
    {
        /// <summary>
        /// Returns the number of assets by given asset type
        /// </summary>
        /// <param name="assetTypeId"></param>
        /// <returns></returns>
        public static int GetPermittedAssetsCount(long assetTypeId,
            IUnitOfWork unitOfWork,
            IAuthenticationService authenticationService)
        {
            var userId = (long)authenticationService.CurrentUser.ProviderUserKey;
            object tmp = unitOfWork.SqlProvider.ExecuteScalar(StoredProcedures.GetPermittedAssetsCount,
                 new SqlParameter[] { 
                        new SqlParameter("@assetTypeId", assetTypeId) { SqlDbType = System.Data.SqlDbType.BigInt },
                        new SqlParameter("@userId", userId) { SqlDbType = System.Data.SqlDbType.BigInt },
                },
                System.Data.CommandType.StoredProcedure);

            int returnValue = 0;
            if (!object.Equals(tmp, null))
                int.TryParse(tmp.ToString(), out returnValue);
            return returnValue;
        }
        
        /// <summary>
        /// Alters a column in table with provided values.
        /// If column is not found, it will be created.
        /// </summary>
        /// <param name="table">Name of table with column to alter</param>
        /// <param name="column">Name of column</param>
        /// <param name="datatype">New data type of column. You can put the data size in brackets.</param>      
        /// <param name="isNull">Can column be null or not</param>
        public static void AlterTable(long dynEntityConfigUid, DynColumn column, IUnitOfWork unitOfWork)
        {
            unitOfWork.SqlProvider.ExecuteNonQuery(StoredProcedures.AlterTable,
                new SqlParameter[] { 
                    new SqlParameter("@dynEntityConfigUid", dynEntityConfigUid),
                    new SqlParameter("@column_name", column.Name),
                    new SqlParameter("@column_datatype", DataTypeService.ConvertToDbDataType(column.DataType)),
                    new SqlParameter("@column_isnull", column.IsNull),
                    new SqlParameter("@column_default", column.IsNull ? string.Empty : column.GetDefaultValue()),
                },
                System.Data.CommandType.StoredProcedure);
        }

        /// <summary>
        /// Checks if this table is exists in database or not
        /// </summary>        
        public static bool IsTableExists(IUnitOfWork unitOfWork, string tableName)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("UnitOfWork");

            var result = unitOfWork.SqlProvider.ExecuteScalar("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName;",
                new SqlParameter[] { new SqlParameter("@tableName", tableName) });
            return int.Parse(result.ToString()) > 0;
        }

        /// <summary>
        /// Returns is column exists in given table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static bool IsColumnExists(IUnitOfWork unitOfWork, string tableName, string columnName)
        {
            var result = unitOfWork.SqlProvider.ExecuteScalar("SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS " +
                "WHERE TABLE_NAME=@tableName AND COLUMN_NAME=@columnName;",
                new SqlParameter[] { 
                    new SqlParameter("@tableName", tableName),
                    new SqlParameter("@columnName", columnName),
                });
            return int.Parse(result.ToString()) > 0;
        }

    }
}
