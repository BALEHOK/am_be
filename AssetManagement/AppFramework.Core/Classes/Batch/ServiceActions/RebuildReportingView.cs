using AppFramework.DataProxy;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AppFramework.Core.Classes.Batch.ServiceActions
{
    internal class RebuildReportingView : BatchAction
    {
        public RebuildReportingView(Entities.BatchAction action)
            : base(action)
        {

        }

        public override void Run()
        {
            var fromAssetTypeUid = long.Parse(this.Parameters["FromAssetType"].ToString());
            var toAssetTypeUid = long.Parse(this.Parameters["ToAssetType"].ToString());
            if (fromAssetTypeUid == 0 || toAssetTypeUid == 0)
                throw new System.ArgumentException();

            var oldConfig = AssetType.GetByUidDb(fromAssetTypeUid);
            var newConfig = AssetType.GetByUidDb(toAssetTypeUid);

            if (oldConfig == null)
                throw new NullReferenceException(string.Format("Cannot retrieve Type with Uid {0}",
                    fromAssetTypeUid));
            if (newConfig == null)
                throw new NullReferenceException(string.Format("Cannot retrieve Type with Uid {0}",
                    toAssetTypeUid));

            var unitOfWork = new UnitOfWork();

            var dynEntityConfigUidParameter = new SqlParameter("DynEntityConfigUid", newConfig.UID);
            var tableNameParameter = new SqlParameter("TableName", newConfig.DBTableName);

            unitOfWork.SqlProvider.ExecuteNonQuery(
                StoredProcedures.RebuildReportingView,
                new IDataParameter[]
                    {
                        dynEntityConfigUidParameter, tableNameParameter
                    },
                CommandType.StoredProcedure);

        }
    }
}