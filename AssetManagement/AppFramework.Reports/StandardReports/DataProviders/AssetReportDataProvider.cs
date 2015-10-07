using AppFramework.Core.Classes;
using AppFramework.Core.Exceptions;
using AppFramework.Reports;
using AppFramework.Reports.Models;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;

namespace AppFramework.Reports.StandardReports.DataProviders
{
    public class AssetReportDataProvider : IReportDataProvider<AssetXtraReport>
    {
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetsService _assetsService;

        public AssetReportDataProvider(
            IAssetTypeRepository assetTypeRepository, 
            IAssetsService assetsService)
        {
            if (assetTypeRepository == null)
                throw new ArgumentNullException();
            _assetTypeRepository = assetTypeRepository;
            if (assetsService == null)
                throw new ArgumentNullException();
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
            return new AssetsContainer
            {
                ReportTitle = assetModel.Name,
                Assets = new List<AssetViewModel> { assetModel }
            };
        }
    }
}