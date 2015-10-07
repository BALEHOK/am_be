using System.Data;
using System.Data.SqlClient;
using AppFramework.Core.ConstantsEnumerators;

namespace AppFramework.Core.Classes.Batch.ServiceActions
{
    internal class RebuildSearchIndexHistory : BatchAction
    {
        public RebuildSearchIndexHistory(Entities.BatchAction batchAction)
            : base(batchAction) { }

        public override void Run()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            unitOfWork.SqlProvider.ExecuteNonQuery(StoredProcedures.ReIndexAll,
                                                   new IDataParameter[] { new SqlParameter("@active", false), },
                                                   CommandType.StoredProcedure);
        }
    }
}