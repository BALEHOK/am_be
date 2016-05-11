using System.Data;
using System.Data.SqlClient;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes.Batch.AssetTypeActions
{
    class RebuildTypeIndex : BatchAction
    {
        public RebuildTypeIndex(Entities.BatchAction batchAction)
            : base(batchAction)
        { }

        public override void Run()
        {
            var dynEntityConfigId = long.Parse(this.Parameters["DynEntityConfigId"].ToString());
            var unitOfWork = new UnitOfWork();
            unitOfWork.SqlProvider.ExecuteNonQuery(
                StoredProcedures.ReIndex,
                new IDataParameter[]
                    {
                        new SqlParameter("@DynEntityConfigId", dynEntityConfigId),
                        new SqlParameter("@active", 1),
                        new SqlParameter("@buildDynEntityIndex", 1)
                    },
                CommandType.StoredProcedure);
        }
    }
}
