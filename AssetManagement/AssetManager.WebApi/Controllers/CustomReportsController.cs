using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
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
        private readonly ICustomReportService<CustomDevExpressReport> _reportService;

        public CustomReportsController(
            ICustomReportService<CustomDevExpressReport> reportService,
            IUnitOfWork unitOfWork)
        {
            if (reportService == null)
                throw new ArgumentNullException("reportService");
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _reportService = reportService;
            _reportFilter = new SearchResultReportFilter(unitOfWork);
        }

        public ActionResult Index(long id, long? assetId = null, long? searchId = null)
        {            
            var report = _reportService.GetReportById(id);
            if (assetId.HasValue)
                report.Filter = string.Format("[id] = {0}", assetId);
            if (searchId.HasValue)
                report.Filter = _reportFilter.GetFilterString(searchId.Value);
            return View("~/Views/Shared/Report.cshtml", report.ReportObject);
        }
    }
}