using System;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.IE;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Entities;

namespace AssetManager.Infrastructure.Services
{
    public class ExportProxy : IExportService
    {
        private readonly ISearchService _searchService;
        private readonly IEnvironmentSettings _envSettings;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetsExporter _assetsExporter;

        public ExportProxy(ISearchService searchService,
            IEnvironmentSettings envSettings,
            IAssetTypeRepository assetTypeRepository,
            IAssetsExporter assetsExporter)
        {
            if (searchService == null)
                throw new ArgumentNullException("searchService");
            _searchService = searchService;

            if (envSettings == null)
                throw new ArgumentNullException("envSettings");
            _envSettings = envSettings;

            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;

            if (assetsExporter == null)
                throw new ArgumentNullException("assetsExporter");
            _assetsExporter = assetsExporter;
        }

        public byte[] ExportToXml(SearchTracking searchTracking, long userId)
        {
            var exportService = GetExportService(searchTracking);

            return exportService.ExportToXml(searchTracking, userId);
        }

        public byte[] ExportToExcel(SearchTracking searchTracking, long userId)
        {
            var exportService = GetExportService(searchTracking);

            return exportService.ExportToExcel(searchTracking, userId);
        }

        public byte[] ExportToTxt(SearchTracking searchTracking, long userId)
        {
            var exportService = GetExportService(searchTracking);

            return exportService.ExportToTxt(searchTracking, userId);
        }

        private IExportService GetExportService(SearchTracking searchTracking)
        {
            var searchType = (SearchType) searchTracking.SearchType;
            IExportService exportService;
            switch (searchType)
            {
                case SearchType.SearchByKeywords:
                    exportService = new ExportSearchResultService(_envSettings, _searchService);
                    break;

                case SearchType.SearchByType:
                    exportService = new ExportByTypeService(_assetTypeRepository, _assetsExporter);
                    break;

                default:
                    throw new NotSupportedException("Unsupported type of export");
            }
            return exportService;
        }
    }
}