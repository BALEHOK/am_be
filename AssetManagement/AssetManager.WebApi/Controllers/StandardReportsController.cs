using System;
using System.Web.Mvc;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Reports;
using AppFramework.Reports.Models;
using AppFramework.Reports.Services;
using AppFramework.Reports.StandardReports.DataProviders;

namespace AssetManager.WebApi.Controllers
{
    // TODO: add security
    [AllowAnonymous]
    public class StandardReportsController : Controller
    {
        private readonly ISearchTracker _searchTracker;
        private readonly ISearchService _searchService;
        private readonly IStandardReportService _reportService;
        private readonly IDataProviderFactory _reportDataProviderFactory;

        public StandardReportsController(
            ISearchTracker searchTracker,
            ISearchService searchService,
            IStandardReportService reportService,
            IDataProviderFactory reportDataProviderFactory)
        {
            if (searchTracker == null)
                throw new ArgumentNullException("searchTracker");
            _searchTracker = searchTracker;

            if (searchService == null)
                throw new ArgumentNullException("searchService");
            _searchService = searchService;

            if (reportService == null)
                throw new ArgumentNullException("reportService");
            _reportService = reportService;

            if (reportDataProviderFactory == null)
                throw new ArgumentNullException("reportDataProviderFactory");
            _reportDataProviderFactory = reportDataProviderFactory;
        }

        public ActionResult SearchResult(Guid searchId, int? reportLayout = null)
        {
            //TODO: get actual current user
            long userId = 1;

            var searchTracking = _searchTracker.GetTrackingBySearchIdUserId(searchId, userId);
            var searchResults = _searchService.GetSearchResultsByTracking(searchTracking, userId);

            var layout = LayoutType.Default;
            if (reportLayout.HasValue)
                layout = (LayoutType) reportLayout;
            var report = _reportService.GetStandardReport(ReportType.SearchResultReport, layout);
            report.DataSource = searchResults.ToModel(string.Format(
                "Search conditions: {0}", searchTracking.VerboseString));
            return View("~/Views/Shared/Report.cshtml", report);
        }

        public ActionResult Asset(long assetTypeId, long assetId)
        {
            //TODO: get actual current user
            long userId = 1;

            var report = _reportService.GetStandardReport(
                ReportType.AssetReport, LayoutType.Default);
            var provider = _reportDataProviderFactory.GetDataProvider<AssetXtraReport>();
            report.DataSource = provider.GetData(assetTypeId, assetId, userId);
            return View("~/Views/Shared/Report.cshtml", report);
        }

        public ActionResult AssetWithChildren(long assetTypeId, long assetId)
        {
            //TODO: get actual current user
            long userId = 1;

            var report = _reportService.GetStandardReport(
                ReportType.AssetsWithChildsReport, LayoutType.Default);
            var provider = _reportDataProviderFactory.GetDataProvider<AssetsWithChildsReport>();
            report.DataSource = provider.GetData(assetTypeId, assetId, userId);
            return View("~/Views/Shared/Report.cshtml", report);
        }
    }
}