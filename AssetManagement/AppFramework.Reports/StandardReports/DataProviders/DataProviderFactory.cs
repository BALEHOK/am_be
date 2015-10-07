using AppFramework.Core.Classes;
using AppFramework.Core.Classes.IE;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.DataProxy;
using AppFramework.Reports;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;

namespace AppFramework.Reports.StandardReports.DataProviders
{
    public class DataProviderFactory : IDataProviderFactory
    {
        private readonly IReportDataProvider<AssetsListXtraReport> _assetsListXtraReportProvider;
        private readonly IReportDataProvider<AssetsWithChildsReport> _assetsWithChildsReportProvider;
        private readonly IReportDataProvider<AssetXtraReport> _assetXtraReportProvider;

        public DataProviderFactory(
            IReportDataProvider<AssetXtraReport> assetXtraReportProvider,
            IReportDataProvider<AssetsWithChildsReport> assetsWithChildsReportProvider,
            IReportDataProvider<AssetsListXtraReport> assetsListXtraReportProvider,
            IReportDataProvider<SearchResultXtraReport> searchResultXtraReportProvider)
        {
            if (assetXtraReportProvider == null)
                throw new ArgumentNullException("assetXtraReportProvider");
            _assetXtraReportProvider = assetXtraReportProvider;
            if (assetsWithChildsReportProvider == null)
                throw new ArgumentNullException("assetsWithChildsReportProvider");
            _assetsWithChildsReportProvider = assetsWithChildsReportProvider;
            if (assetsListXtraReportProvider == null)
                throw new ArgumentNullException("assetsListXtraReportProvider");
            _assetsListXtraReportProvider = assetsListXtraReportProvider;           
        }

        public IReportDataProvider<T> GetDataProvider<T>() where T : XtraReport
        {
            if (typeof(T) == typeof(AssetXtraReport))
                return _assetXtraReportProvider as IReportDataProvider<T>;
            if (typeof(T) == typeof(AssetsWithChildsReport))
                return _assetsWithChildsReportProvider as IReportDataProvider<T>;
            if (typeof(T) == typeof(AssetsListXtraReport))
                return _assetsListXtraReportProvider as IReportDataProvider<T>;
            throw new NotSupportedException(typeof(T).Name);
        }        
    }
}