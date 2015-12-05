using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.DAL;
using AppFramework.Core.DAL.Adapters;
using AppFramework.DataProxy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetManager.Infrastructure.Services
{
    public class FaqService : IFaqService
    {
        private readonly IAssetsService _assetsService;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAttributeValueFormatter _attributeValueFormatter;
        private readonly DynColumnAdapter _columnAdapter;
        private readonly IDynamicListsService _dynamicListsService;
        private readonly ITableProvider _tableProvider;
        private readonly IUnitOfWork _unitOfWork;

        public FaqService(
            IUnitOfWork unitOfWork,
            ITableProvider tableProvider,
            IAssetTypeRepository assetTypeRepository,
            IAssetsService assetsService,
            IAttributeValueFormatter attributeValueFormatter,
            IAttributeRepository attributeRepository,
            IDynamicListsService dynamicListsService)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
            if (attributeValueFormatter == null)
                throw new ArgumentNullException("attributeValueFormatter");
            _attributeValueFormatter = attributeValueFormatter;
            if (tableProvider == null)
                throw new ArgumentNullException("tableProvider");
            _tableProvider = tableProvider;
            if (dynamicListsService == null)
                throw new ArgumentNullException("dynamicListsService");
            _dynamicListsService = dynamicListsService;
        }

        /// <summary>
        /// Get items for FAQ section
        /// </summary>
        /// <param name="cultureName">Culture name from Languege table</param>
        /// <returns></returns>
        public IEnumerable<Asset> GetFaqItems(System.Globalization.CultureInfo culture = null, int itemsNumber = 1000)
        {
            var currentAT = _assetTypeRepository.GetPredefinedAssetType(PredefinedEntity.Faq);
            if (!currentAT.Attributes.Any(a => a.DBTableFieldName == "Language"))
                throw new Exception("Language attribute missing for FAQ");

            var options = new Dictionary<string, string>() { { AttributeNames.ActiveVersion, bool.TrueString } };
            var rows = _tableProvider.GetRows(currentAT.Base, options, 0, itemsNumber);
            var faqItems = (from row in rows
                            where row != null
                            select new Asset( 
                                    currentAT,
                                    row,
                                    _attributeValueFormatter,
                                    _assetTypeRepository,
                                    _assetsService,
                                    _unitOfWork,
                                    _dynamicListsService)).ToList(); // TODO: refactor Asset class to POCO

            if (culture != null)
                faqItems = faqItems.Where(i => i["Language"].Value == culture.Name).ToList();

            return faqItems;
        }

        public long GetFaqAssetTypeId()
        {
            var faqType = _assetTypeRepository.GetPredefinedAssetType(PredefinedEntity.Faq);
            return faqType.ID;
        }
    }
}
