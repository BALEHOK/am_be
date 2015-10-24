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

            var childAssetPointers = _unitOfWork.GetChildAssets(assetTypeId);
            foreach (var assetsPointer in childAssetPointers)
            {
                var childAssetType = _assetTypeRepository.GetById(assetsPointer.DynEntityConfigId);
                var childAssets = from ca in _assetsService.GetAssetsByAssetTypeAndUser(childAssetType, currentUserId)
                                  let attribute = asset.Attributes
                                     .SingleOrDefault(att => att.GetConfiguration().UID == assetsPointer.DynEntityAttribConfigUid)
                                  where attribute != null && attribute.ValueAsId == asset.ID
                                  select ca;

                foreach (var ca in childAssets)
                {
                    var childAssetModel = new AssetViewModel
                    {
                        Name = string.Format("{0} ({1})",
                        ca.Name, ca.GetConfiguration().Name)
                    };

                    Func<AssetAttribute, bool> descriptionPredicate =
                        a => a.GetConfiguration().IsDescription && !string.IsNullOrEmpty(a.Value);
                    Func<AssetAttribute, bool> fallbackOnePredicate =
                        a => a.GetConfiguration().IsShownInGrid && !string.IsNullOrEmpty(a.Value);
                    Func<AssetAttribute, bool> fallbackTwoPredicate =
                        a => a.GetConfiguration().IsShownOnPanel && !string.IsNullOrEmpty(a.Value);

                    var attributes = new List<AssetAttribute>();
                    if (ca.Attributes.Any(descriptionPredicate))
                        attributes = ca.Attributes.Where(descriptionPredicate).ToList();
                    else if (ca.Attributes.Any(fallbackOnePredicate))
                        attributes = ca.Attributes.Where(fallbackOnePredicate).ToList();
                    else if (ca.Attributes.Any(fallbackTwoPredicate))
                        attributes = ca.Attributes.Where(fallbackTwoPredicate).ToList();

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