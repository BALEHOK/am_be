using System;
using System.Web.Mvc;
using AppFramework.Reports.Services;
using AppFramework.Reports.CustomReports;
using AppFramework.DataProxy;

namespace AssetManager.WebApi.Controllers
{
    // TODO: add security
    [AllowAnonymous]
    public class CustomReportsController : Controller
    {
        private readonly SearchResultReportFilter _reportFilter;
        private readonly ICustomReportService _reportService;

        public CustomReportsController(
            ICustomReportService reportService,
            IUnitOfWork unitOfWork)
        {
            if (reportService == null)
                throw new ArgumentNullException("reportService");
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _reportService = reportService;
            _reportFilter = new SearchResultReportFilter(unitOfWork);
        }

        public ActionResult Index(long id, long? assetId = null, Guid? searchId = null)
        {            
            var reportData = _reportService.GetReportById(id);
            var report = _reportService.CreateReportView(reportData);
            if (assetId.HasValue)
                report.FilterString = string.Format("[id] = {0}", assetId);
            if (searchId.HasValue)
                report.FilterString = _reportFilter.GetFilterString(searchId.Value);
            return View("~/Views/Shared/Report.cshtml", report);
        }
    }
}