using AppFramework.Core.Classes;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AssetManager.Infrastructure.Models.TypeModels;

namespace AssetManager.Infrastructure.Services
{
    public class TaxonomyService : ITaxonomyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetTypeRepository _assetTypeRepository;

        public TaxonomyService(
            IUnitOfWork unitOfWork,
            IAssetTypeRepository assetTypeRepository)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
        }

        public TaxonomyModel GetTaxonomyByAssetTypeId(long assetTypeId)
        {
            var assetType = _assetTypeRepository.GetById(assetTypeId);

            var item = _unitOfWork.TaxonomyItemRepository
                .GetTaxonomyItemsByAssetTypeId(assetTypeId)
                .SingleOrDefault(ti => ti.Taxonomy.IsCategory); // show only category path

            var assetTypeModel = new AssetTypeModel
                {
                    DisplayName = assetType.NameInvariant,
                    Id = assetType.ID
                };

            return item != null
                ? GetTaxonomyPath(item, null, assetTypeModel)
                : new TaxonomyModel
                    {
                        AssetType = assetTypeModel
                    };
        }

        private TaxonomyModel GetTaxonomyPath(
            AppFramework.Entities.TaxonomyItem item,
            TaxonomyModel childModel = null,
            AssetTypeModel assetTypeModel = null)
        {
            var model = new TaxonomyModel
            {
                Name = item.Name,
                Child = childModel,
                AssetType = assetTypeModel
            };

            if (item.ParentItem == null)
                return model;

            return GetTaxonomyPath(item.ParentItem, model);
        }
    }
}