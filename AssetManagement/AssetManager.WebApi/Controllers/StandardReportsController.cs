using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using AppFramework.Reports.Services;
using AppFramework.Reports.CustomReports;
using AppFramework.Reports;
using AppFramework.Reports.Models;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Reports.StandardReports.DataProviders;

namespace AssetManager.WebApi.Controllers
{
    // TODO: add security
    [AllowAnonymous]
    public class StandardReportsController : Controller
    {
        private readonly ISearchService _searchService;
        private readonly IStandardReportService _reportService;
        private readonly IDataProviderFactory _reportDataProviderFactory;

        public StandardReportsController(
            ISearchService searchService,
            IStandardReportService reportService,
            IDataProviderFactory reportDataProviderFactory)
        {
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

            var searchTracking = _searchService.GetTrackingBySearchId(searchId, userId);
            var searchResults = _searchService.GetSearchResultsBySearchId(searchId, userId);

            var layout = ReportLayout.Default;
            if (reportLayout.HasValue)
                layout = (ReportLayout)reportLayout;
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
                ReportType.AssetReport, ReportLayout.Default);
            var provider = _reportDataProviderFactory.GetDataProvider<AssetXtraReport>();
            report.DataSource = provider.GetData(assetTypeId, assetId, userId);
            return View("~/Views/Shared/Report.cshtml", report);
        }

        public ActionResult AssetWithChildren(long assetTypeId, long assetId)
        {
            //TODO: get actual current user
            long userId = 1;

            var report = _reportService.GetStandardReport(
                ReportType.AssetsWithChildsReport, ReportLayout.Default);
            var provider = _reportDataProviderFactory.GetDataProvider<AssetsWithChildsReport>();
            report.DataSource = provider.GetData(assetTypeId, assetId, userId);
            return View("~/Views/Shared/Report.cshtml", report);
        }
    }
}