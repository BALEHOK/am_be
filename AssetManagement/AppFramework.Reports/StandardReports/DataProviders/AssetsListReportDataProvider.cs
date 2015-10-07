using System.Linq;
using AppFramework.Core.Classes;
using AppFramework.Core.Exceptions;
using AppFramework.Reports;
using AppFramework.Reports.Models;
using System;
using System.Collections.Generic;
using DevExpress.XtraReports.UI;

namespace AppFramework.Reports.StandardReports.DataProviders
{
    public class AssetsListReportDataProvider : IReportDataProvider<AssetsListXtraReport>
    {
        private readonly IAssetsService _assetsService;
        private readonly IAssetTypeRepository _assetTypeRepository;

        public AssetsListReportDataProvider( 
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
            var assets = _assetsService.GetAssetsByAssetTypeAndUser(assetType, currentUserId).ToList();
            var dataSource = new List<AssetViewModel>(assets.Count);
            foreach (var asset in assets)
            {
                var assetModel = new AssetViewModel {Name = asset.Name};
                foreach (var attribute in asset.AttributesPublic)
                {
                    assetModel.Attributes.Add(new AssetAttributeViewModel
                    {
                        Name = attribute.Configuration.NameLocalized,
                        Value = attribute.Value
                    });
                }
                dataSource.Add(assetModel);
            }
            return new AssetsContainer {Assets = dataSource, ReportTitle = assetType.Name};
        }
    }
}