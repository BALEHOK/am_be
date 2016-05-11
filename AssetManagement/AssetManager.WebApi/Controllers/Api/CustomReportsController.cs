using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AppFramework.Core.Classes.Extensions;
using AppFramework.Reports.Services;
using AssetManager.Infrastructure.Extensions;
using AssetManager.Infrastructure.Helpers;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Services;
using Common.Logging;
using WebApi.OutputCache.V2;

namespace AssetManager.WebApi.Controllers.Api
{
    /// <summary>
    /// Custom reports API
    /// </summary>
    [RoutePrefix("api/reports/custom")]
    public class CustomReportsController : ApiController
    {
        private readonly ICustomReportService _customReportService;
        private readonly IAssetTypeService _assetTypeService;
        private readonly ILog _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public CustomReportsController(
            ICustomReportService customReportService,
            IAssetTypeService assetTypeService,
            ILog logger)
        {
            _customReportService = customReportService;
            _assetTypeService = assetTypeService;
            _logger = logger;
        }

        /// <summary>
        /// Returns a list of all published custom reports
        /// </summary>
        /// <returns></returns>
        [Route("list"), Route(""), HttpGet]
        public List<CustomReportModel> ReportsList()
        {
            var userId = User.GetId();
            var reports = _customReportService
                .GetAllReports(userId);

            var relatedAssetTypes =
                _assetTypeService.GetAssetTypesByIds(userId, reports
                    .Select(r => r.DynEntityConfigId.GetValueOrDefault())
                    .Distinct())
                    .ToDictionary(t => t.DynEntityConfigId, t => t.NameLocalized());

            return reports.Select(r => r.ToModel(relatedAssetTypes))
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
            var assetType = _assetTypeService.GetAssetType(User.GetId(), assetTypeId);

            return _customReportService
                .GetReportsByAssetTypeId(assetTypeId, User.GetId())
                .Select(r => r.ToModel(new Dictionary<long, string> {{assetTypeId, assetType.DisplayName}}))
                .ToList();
        }

        /// <summary>
        /// Publish custom report
        /// </summary>
        /// <param name="reportName">Report name</param>
        /// <param name="assetTypeId">Asset type Id</param>
        [Route("create"), HttpPut]
        public long CreateReport(long assetTypeId, string reportName)
        {
            var report = _customReportService.CreateReport(assetTypeId, reportName, User.GetId());
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
            _customReportService.DeleteReport(reportId, User.GetId());
        }
    }
}