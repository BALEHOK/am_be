using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Calculation;
using AppFramework.Core.Classes.Barcode;
using AppFramework.DataProxy;
using System;
using AppFramework.Core.Classes.SearchEngine;
using Common.Logging;

namespace AppFramework.Core.Classes.Batch
{
    public interface IBatchActionFactory
    {
        /// <summary>
        /// Gets the action of the by action type.
        /// </summary>
        BatchAction GetAction(Entities.BatchAction action);
    }

    public class BatchActionFactory : IBatchActionFactory
    {
        private readonly IAssetsService _assetsService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly ILog _logger;
        private readonly IBarcodeProvider _barcodeProvider;
        private readonly IAttributeCalculator _attributeCalculator;
        private readonly IIndexationService _indexationService;

        public BatchActionFactory(
            IAssetsService assetsService, 
            IUnitOfWork unitOfWork, 
            IAssetTypeRepository assetTypeRepository,
            IBarcodeProvider barcodeProvider,
            IAttributeCalculator attributeCalculator,
            IIndexationService indexationService,
            ILog logger)
	    {
	        if (assetsService == null)
	            throw new ArgumentNullException("assetsService");
	        _assetsService = assetsService;
	        if (unitOfWork == null)
	            throw new ArgumentNullException("unitOfWork");
	        _unitOfWork = unitOfWork;
	        if (assetTypeRepository == null)
	            throw new ArgumentNullException("assetTypeRepository");
	        _assetTypeRepository = assetTypeRepository;
	        if (logger == null)
	            throw new ArgumentNullException("logger");
	        _logger = logger;
	        if (barcodeProvider == null)
	            throw new ArgumentNullException("barcodeProvider");
	        _barcodeProvider = barcodeProvider;
	        if (attributeCalculator == null)
	            throw new ArgumentNullException("attributeCalculator");
	        _attributeCalculator = attributeCalculator;
            if (indexationService == null)
                throw new ArgumentNullException("indexationService");
            _indexationService = indexationService;
	    }

		/// <summary>
		/// Gets the action of the by action type.
		/// </summary>
		/// <param name="actionType">Type of the action.</param>
		/// <param name="action">The BatchAction entity.</param>
		/// <returns></returns>
        public BatchAction GetAction(Entities.BatchAction action)
		{
			if (action == null)
				throw new ArgumentNullException("BatchAction data is not provided");
		    
			BatchAction act = null;
			switch ((BatchActionType)action.ActionType)
			{
				case BatchActionType.CreateAssetsRevision:
					act = new AssetTypeActions.CreateAssetsRevisions(
                        action, _assetTypeRepository, _assetsService, _unitOfWork);
					break;
				case BatchActionType.UpdateAssetsReferences:
                    act = new AssetTypeActions.UpdateAssetsReferences(
                        action, _assetsService, _assetTypeRepository);
					break;
				case BatchActionType.TaxonomyBatch:
					act = new TaxonomyActions.TaxonomyBatch(action, _unitOfWork);
					break;
				case BatchActionType.PublishAssetType:
					act = new AssetTypeActions.PublishAssetType(action, _unitOfWork);
					break;
				case BatchActionType.ImportAssets:
                    act = new AssetActions.ImportAssets(
                        action, 
                        _unitOfWork, 
                        _assetsService, 
                        _assetTypeRepository, 
                        _barcodeProvider,
                        _logger);
					break;
				case BatchActionType.RebuildSearchIndexActive:
					act = new ServiceActions.RebuildSearchIndexActive(action);
					break;
				case BatchActionType.RebuildSearchIndexHistory:
					act = new ServiceActions.RebuildSearchIndexHistory(action);
					break;
				case BatchActionType.MoveToLocation:
					act = new AssetActions.LocationMove(action, _assetsService);
					break;
				case BatchActionType.UpdateAssets:
					act = new AssetActions.UpdateAssets(action, _unitOfWork, _assetTypeRepository, _assetsService);
					break;
                case BatchActionType.RecalculateAssets:
                    act = new AssetActions.RecalculateAssets(action, _unitOfWork, _assetsService,
                        _assetTypeRepository, _attributeCalculator, _indexationService);
                    break;
                case BatchActionType.SynkAssets:
					act = new AssetActions.SynkAssets(action, _unitOfWork, _assetsService, _assetTypeRepository, 
                        _barcodeProvider);
					break;
				case BatchActionType.RebuildReportingView:
					act = new ServiceActions.RebuildReportingView(action);
					break;
				default:
					break;
			}
			return act;
		}
	}
}
