using System.Net;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.ConstantsEnumerators;
using System;
using System.Collections.Generic;
using System.Data;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes.Batch.AssetTypeActions
{

    /// <summary>
    /// This action create new revisions for all assets of asset type given as fromAssetType parameter and link it to asset type with Uid equal toAssetType parameter
    /// </summary>
    public class CreateAssetsRevisions : BatchAction
    {
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetsService _assetsService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAssetsRevisions(Entities.BatchAction batchAction,
            IAssetTypeRepository assetTypeRepository,
            IAssetsService assetsService,
            IUnitOfWork unitOfWork)
            : base(batchAction)
        {
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public override void Run()
        {
            var fromAssetTypeUid = long.Parse(this.Parameters["FromAssetType"].ToString());
            var toAssetTypeUid = long.Parse(this.Parameters["ToAssetType"].ToString());
            if (fromAssetTypeUid == 0 || toAssetTypeUid == 0)
                throw new System.ArgumentException();

            var oldConfig = _assetTypeRepository.GetByUid(fromAssetTypeUid);
            var newConfig = _assetTypeRepository.GetByUid(toAssetTypeUid);

            if (oldConfig == null)
                throw new NullReferenceException(string.Format("Cannot retrieve Type with Uid {0}",
                    fromAssetTypeUid));
            if (newConfig == null)
                throw new NullReferenceException(string.Format("Cannot retrieve Type with Uid {0}", 
                    toAssetTypeUid));

            var assets = new List<Asset>(_assetsService.GetAssetsByAssetType(oldConfig)).AsReadOnly();
            var udfTable = new DAL.DynEntityIdsTableType("@entities");
            foreach (var asset in assets)
            {
                udfTable.AddEntity(asset.UID, asset.ID, asset.GetConfiguration().UID);
            }
            _unitOfWork.SqlProvider.ExecuteNonQuery(StoredProcedures.CreateNewRevision,
                                                   new IDataParameter[]
                                                       {
                                                           udfTable.SqlParameter
                                                       },
                                                   CommandType.StoredProcedure);            
        }
    }
}
