using System;
using System.Collections.Generic;
using AppFramework.Core.Calculation;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.DataProxy;
using LinqKit;

namespace AppFramework.Core.Classes.Batch.AssetActions
{
    /// <summary>
    /// Calculates formula attributes values for all assets of given type
    /// 
    /// Parameters:
    /// TypeUid - asset type Uid
    /// </summary>
    public class RecalculateAssets : BatchAction
    {
        private readonly IAssetsService _assetsService;
        private readonly IAttributeCalculator _attributeCalculator;
        private readonly IAssetTypeRepository _assetTypeRepository;

        public RecalculateAssets(Entities.BatchAction batchAction,
            IUnitOfWork unitOfWork,
            IAssetsService assetsService,
            IAssetTypeRepository assetTypeRepository,
            IAttributeCalculator attributeCalculator)
            : base(batchAction)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
            if (attributeCalculator == null)
                throw new ArgumentNullException("attributeCalculator");
            _attributeCalculator = attributeCalculator;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
        }

        public override void Run()
        {
            var typeUid = long.Parse(Parameters["ToAssetType"]);

            var assetType = _assetTypeRepository.GetByUid(typeUid);
            var assets = _assetsService.GetAssetsByParameters(assetType, new Dictionary<string, string>()
            {
                {AttributeNames.ActiveVersion, bool.TrueString}
            });

//            throw new NotImplementedException();

            assets.ForEach(asset =>
            {
                var calculatedAsset = _attributeCalculator.PostCalculateAsset(asset);
                _assetsService.InsertAsset(calculatedAsset);
            });
        }
    }
}