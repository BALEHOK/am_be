using LinqKit;

namespace AppFramework.Core.Classes.SearchEngine
{
    using AppFramework.Core.ConstantsEnumerators;
    using AppFramework.DataProxy;
    using AppFramework.Entities;
    using System;
    using System.Linq;

    /// <summary>
    /// DynEntityIndex persistense operatons
    /// </summary>
    public class IndexationService : IIndexationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetsService _assetsService;

        public IndexationService(
            IUnitOfWork unitOfWork, 
            IAssetTypeRepository assetTypeRepository, 
            IAssetsService assetsService)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("IUnitOfWork");
            _unitOfWork = unitOfWork;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
        }

        /// <summary>
        /// Adds asset to fast indexing system (for future using in stock scanner and inventarisation module)
        /// </summary>
        /// <param name="asset"></param>
        public void UpdateIndex(Asset asset)
        {
            long entityId = asset.ID;
            long configId = asset.GetConfiguration().ID;
            if (entityId == 0)
                throw new ArgumentException("Cannot add to index: asset is not persistent");

            IndexAsset(asset);

            // add to context index
            if (asset.GetConfiguration().IsContextIndexed)
            {
                // save old attributes to history
                long assetTypeUid = asset.GetConfiguration().UID;
                var contextsToUpdate = _unitOfWork.DynEntityContextAttributesValuesRepository.Get(
                    d => d.DynEntityUid == asset.UID && d.DynEntityConfigUid == assetTypeUid).ToList();

                contextsToUpdate.ForEach(c =>
                {
                    c.IsActive = false;
                    _unitOfWork.DynEntityContextAttributesValuesRepository.Update(c);
                });

                if (asset.Attributes.Any(attr => attr.GetConfiguration().ContextId.HasValue))
                    IndexFactory<DynEntityContextAttributesValues>.GenerateIndexEntities(asset)
                        .ForEach(c => _unitOfWork.DynEntityContextAttributesValuesRepository.Insert(c));
            }

            var index = _unitOfWork.DynEntityIndexRepository.SingleOrDefault(i => i.DynEntityId == entityId && i.DynEntityConfigId == configId);
            if (index != null && asset[AttributeNames.ActiveVersion].Value == false.ToString())
            {
                // remove deleted asset from index
                _unitOfWork.DynEntityIndexRepository.Delete(index);
            }
            else
            {
                bool isInsert = false;
                if (index == null)
                {
                    index = new DynEntityIndex()
                    {
                        DynEntityId = entityId,
                        DynEntityConfigId = configId
                    };
                    isInsert = true;
                }

                index.Barcode = asset[AttributeNames.Barcode] != null 
                    ? asset[AttributeNames.Barcode].Value : null;
                index.LocationId = asset[AttributeNames.LocationId] != null 
                    ? asset[AttributeNames.LocationId].ValueAsId : null;
                index.DepartmentId = asset[AttributeNames.DepartmentId] != null 
                    ? asset[AttributeNames.DepartmentId].ValueAsId : null;
                index.UserId = asset[AttributeNames.UserId] != null 
                    ? asset[AttributeNames.UserId].ValueAsId : null;
                index.OwnerId = asset[AttributeNames.OwnerId] != null 
                    ? asset[AttributeNames.OwnerId].ValueAsId : null;
                index.Name = asset[AttributeNames.Name] != null 
                    ? asset[AttributeNames.Name].Value : null;

                if (isInsert)
                {
                    _unitOfWork.DynEntityIndexRepository.Insert(index);
                }
                else
                {
                    _unitOfWork.DynEntityIndexRepository.Update(index);
                }
            }
            _unitOfWork.Commit();
        }

        void IndexAsset(Asset asset)
        {
            if (asset.ID == 0)
                throw new ArgumentException("Cannot add to index: asset is not persistent");

            long assetTypeId = asset.GetConfiguration().ID;
            var outdatedEntity = _unitOfWork.IndexActiveDynEntitiesRepository.SingleOrDefault(i =>
                i.DynEntityId == asset.ID &&
                i.DynEntityConfigId == assetTypeId);

            if (outdatedEntity != null)
            {
                _unitOfWork.IndexHistoryDynEntitiesRepository.Insert(cloneToHistory(outdatedEntity));
                _unitOfWork.IndexActiveDynEntitiesRepository.Delete(outdatedEntity);
            }

            if (!Convert.ToBoolean(asset[AttributeNames.ActiveVersion].Value))
                return;

            var indexEntity = IndexFactory<IndexActiveDynEntities>.GenerateIndexEntity(
                asset, 
                _unitOfWork,
                _assetTypeRepository,
                _assetsService);
            _unitOfWork.IndexActiveDynEntitiesRepository.Insert(indexEntity);
            _unitOfWork.Commit();
        }
        
        private static IndexHistoryDynEntities cloneToHistory(IndexActiveDynEntities sourceEntity)
        {
            var map = AutoMapper.Mapper.FindTypeMapFor<IndexActiveDynEntities, IndexHistoryDynEntities>();
            if (map == null)
                AutoMapper.Mapper.CreateMap<IndexActiveDynEntities, IndexHistoryDynEntities>();
            var historyEntity = AutoMapper.Mapper.Map<IndexActiveDynEntities, IndexHistoryDynEntities>(sourceEntity);           
            return historyEntity;
        }
    }
}
