using AppFramework.Core.Classes;
using AppFramework.Core.Exceptions;
using AppFramework.DataProxy;
using AppFramework.Reports;
using AppFramework.Reports.Models;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppFramework.Reports.StandardReports.DataProviders
{
    public class AssetWithChildsReportDataProvider : IReportDataProvider<AssetsWithChildsReport>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetsService _assetsService;

        public AssetWithChildsReportDataProvider(
            IAssetTypeRepository assetTypeRepository,
            IAssetsService assetsService,
            IUnitOfWork unitOfWork)
        {
            if (assetTypeRepository == null)
                throw new ArgumentNullException();
            if (assetsService == null)
                throw new ArgumentNullException();
            if (unitOfWork == null)
                throw new ArgumentNullException();
            _unitOfWork = unitOfWork;
            _assetTypeRepository = assetTypeRepository;
            _assetsService = assetsService;
        }

        public AssetsContainer GetData(long assetTypeId, long assetId, long currentUserId)
        {
            var assetType = _assetTypeRepository.GetById(assetTypeId);
            var asset = _assetsService.GetAssetById(assetId, assetType);

            var assetModel = new AssetViewModel { Name = asset.Name };
            foreach (var attribute in asset.AttributesPublic)
            {
                assetModel.Attributes.Add(new AssetAttributeViewModel
                {
                    Name = attribute.Configuration.NameLocalized,
                    Value = attribute.Value
                });
            }

            var realtedAssetTypeDefinitions = _unitOfWork.GetRelatedAssetTypes(assetTypeId, currentUserId);
            foreach (var typeDef in realtedAssetTypeDefinitions)
            {
                var relatedAssetType = _assetTypeRepository.GetById(
                    typeDef.DynEntityConfigId);

                var allAssetsOfRelatedAssetType = _assetsService.GetAssetsByAssetTypeAndUser(
                    relatedAssetType, currentUserId);

                var relatedAssets = allAssetsOfRelatedAssetType
                    .Where(a => a.Attributes.Any(
                        attr => attr.Configuration.ID == typeDef.DynEntityAttribConfigId
                                && attr.ValueAsId == asset.ID))
                    .ToList();

                foreach (var relatedAsset in relatedAssets)
                {
                    var childAssetModel = new AssetViewModel
                    {
                        Name = string.Format("{0} ({1})",
                        relatedAsset.Name, relatedAsset.GetConfiguration().Name)
                    };

                    Func<AssetAttribute, bool> descriptionPredicate =
                        a => a.GetConfiguration().IsDescription && !string.IsNullOrEmpty(a.Value);
                    Func<AssetAttribute, bool> fallbackOnePredicate =
                        a => a.GetConfiguration().IsShownInGrid && !string.IsNullOrEmpty(a.Value);
                    Func<AssetAttribute, bool> fallbackTwoPredicate =
                        a => a.GetConfiguration().IsShownOnPanel && !string.IsNullOrEmpty(a.Value);

                    var attributes = new List<AssetAttribute>();
                    if (relatedAsset.Attributes.Any(descriptionPredicate))
                        attributes = relatedAsset.Attributes.Where(descriptionPredicate).ToList();
                    else if (relatedAsset.Attributes.Any(fallbackOnePredicate))
                        attributes = relatedAsset.Attributes.Where(fallbackOnePredicate).ToList();
                    else if (relatedAsset.Attributes.Any(fallbackTwoPredicate))
                        attributes = relatedAsset.Attributes.Where(fallbackTwoPredicate).ToList();

                    foreach (var attr in attributes)
                    {
                        childAssetModel.Attributes.Add(new AssetAttributeViewModel
                        {
                            Name = attr.GetConfiguration().NameLocalized,
                            Value = attr.Value
                        });
                    }
                    assetModel.ChildAssets.Add(childAssetModel);
                }
            }
            return new AssetsContainer
            {
                ReportTitle = assetModel.Name,
                Assets = new List<AssetViewModel> { assetModel }
            };
        }
    }
}