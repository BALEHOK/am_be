using AppFramework.Core.Classes;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.Core.Classes.Extensions;
using AssetManager.Infrastructure.Models.TypeModels;
using TaxonomyItem = AppFramework.Entities.TaxonomyItem;

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

            var items = _unitOfWork.TaxonomyItemRepository
                .GetTaxonomyItemsByAssetTypeId(assetTypeId)
                .Where(ti => ti.Taxonomy.IsCategory) // show only category path
                .ToList();

            var taxonomyModel = new TaxonomyModel
            {
                AssetType = new AssetTypeModel
                {
                    DisplayName = assetType.NameInvariant.Localized(),
                    Id = assetType.ID
                },
                TaxonomyPath = items.Count > 0
                    ? items.Select(i => GetTaxonomyPath(i))
                    : new List<TaxonomyPathModel>(0)
            };


            return taxonomyModel;
        }

        private TaxonomyPathModel GetTaxonomyPath(
            TaxonomyItem item,
            TaxonomyPathModel childModel = null)
        {
            var model = new TaxonomyPathModel
            {
                Name = item.Name.Localized(),
                Child = childModel
            };

            if (item.ParentItem == null)
                return model;

            return GetTaxonomyPath(item.ParentItem, model);
        }
    }
}