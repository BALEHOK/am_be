using System.Data;
using System.Data.SqlClient;
using AppFramework.Core.ConstantsEnumerators;
namespace AppFramework.Core.Classes.Batch.ServiceActions
{
    internal class RebuildSearchIndexActive : BatchAction
    {
        public RebuildSearchIndexActive(Entities.BatchAction batchAction)
            : base(batchAction) { }

        public override void Run()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            unitOfWork.SqlProvider.ExecuteNonQuery(StoredProcedures.ReIndexAll,
                                                   new IDataParameter[]
                                                       {
                                                           new SqlParameter("@active", 1),
                                                           new SqlParameter("@buildDynEntityIndex", 1)
                                                       },
                                                   CommandType.StoredProcedure);
        }
    }
}