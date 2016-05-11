using System;
using System.Collections.Generic;
using AppFramework.Core.Calculation;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.DAL;
using AppFramework.Core.DAL.Adapters;
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
        private readonly IIndexationService _indexationService;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly DynTableProvider _tableProvider;

        public RecalculateAssets(Entities.BatchAction batchAction,
            IUnitOfWork unitOfWork,
            IAssetsService assetsService,
            IAssetTypeRepository assetTypeRepository,
            IAttributeCalculator attributeCalculator,
            IIndexationService indexationService)
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

            if (indexationService == null)
                throw new ArgumentNullException("indexationService");
            _indexationService = indexationService;

            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;

            var dtService = new DataTypeService(unitOfWork);
            var columnAdapter = new DynColumnAdapter(dtService);
            _tableProvider = new DynTableProvider(unitOfWork, columnAdapter);
        }

        public override void Run()
        {
            var typeUid = long.Parse(Parameters["ToAssetType"]);

            var assetType = _assetTypeRepository.GetByUid(typeUid);
            var assets = _assetsService.GetAssetsByParameters(assetType, new Dictionary<string, string>()
            {
                {AttributeNames.ActiveVersion, bool.TrueString}
            });

            assets.ForEach(asset =>
            {
                asset = _attributeCalculator.PostCalculateAsset(asset);
                _tableProvider.UpdateAsset(asset);
                _attributeCalculator.CalculateDependencies(asset);
                _indexationService.UpdateIndex(asset);
            });
        }
    }
}