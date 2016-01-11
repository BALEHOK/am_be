using System;
using System.Linq;
using AppFramework.DataProxy;
using AppFramework.Entities;
using LinqKit;

namespace AppFramework.Core.Classes.SearchEngine
{
    /// <summary>
    /// DynEntityIndex persistense operatons
    /// </summary>
    public class IndexationService : IIndexationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexationService(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Adds asset to fast indexing system (for future using in stock scanner and inventarisation module)
        /// </summary>
        /// <param name="asset"></param>
        public void UpdateIndex(Asset asset)
        {
            var entityId = asset.ID;
            if (entityId == 0)
                throw new ArgumentException("Cannot add to index: asset is not persistent");

            _unitOfWork.ReIndexAsset(asset.UID, asset.ID, asset.DynEntityConfigUid, asset.GetConfiguration().ID);

            // add to context index
            if (asset.GetConfiguration().IsContextIndexed)
            {
                // save old attributes to history
                var assetTypeUid = asset.GetConfiguration().UID;
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
            _unitOfWork.Commit();
        }
    }
}