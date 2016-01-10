using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AppFramework.Reports.Services;
using AssetManager.Infrastructure.Models;
using WebApi.OutputCache.V2;
using AssetManager.Infrastructure.Helpers;
using Common.Logging;

namespace AssetManager.WebApi.Controllers.Api
{
    /// <summary>
    /// Custom reports API
    /// </summary>
    [RoutePrefix("api/reports/custom")]
    public class CustomReportsController : ApiController
    {
        private readonly ICustomReportService _customReportService;
        private readonly ILog _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customReportService"></param>
        public CustomReportsController(
            ICustomReportService customReportService,
            ILog logger)
        {
            _customReportService = customReportService;
            _logger = logger;
        }

        /// <summary>
        /// Returns a list of all published custom reports
        /// </summary>
        /// <returns></returns>
        [Route("list"), Route(""), HttpGet]
        public List<CustomReportModel> ReportsList()
        {
            return _customReportService
                .GetAllReports()
                .Select(r => r.ToModel())
                .ToList();
        }

        /// <summary>
        /// Returns a list of all published custom reports
        /// </summary>
        /// <returns></returns>
        [Route("{assetTypeId}"), HttpGet]
        [CacheOutput(ServerTimeSpan = 100, ClientTimeSpan = 100)]
        public List<CustomReportModel> GetReportsByAssetTypeId(long assetTypeId)
        {
            return _customReportService
                .GetReportsByAssetTypeId(assetTypeId)
                .Select(r => r.ToModel())
                .ToList();
        }

        /// <summary>
        /// Publish custom report
        /// </summary>
        /// <param name="name">Report name</param>
        /// <param name="fileName">Report template file name</param>
        /// <param name="typeId">Asset type Id</param>
        [Route("create"), HttpPut]
        public long CreateReport(long assetTypeId, string reportName)
        {
            var report = _customReportService.CreateReport(assetTypeId, reportName);
            return report.ReportUid;
        }

        /// <summary>
        /// Delete custom report 
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        [Route("delete"), HttpDelete]
        public void DeleteReport(long reportId)
        {
            _customReportService.DeleteReport(reportId);
        }
    }
}