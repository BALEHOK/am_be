using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AppFramework.Reports.CustomReports;
using AppFramework.Reports.Services;
using AssetManager.Infrastructure.Models;
using WebApi.OutputCache.V2;
using AssetManager.Infrastructure.Helpers;

namespace AssetManager.WebApi.Controllers.Api
{
    /// <summary>
    /// Custom reports API
    /// </summary>
    [RoutePrefix("api/reports/custom")]
    public class CustomReportsController : ApiController
    {
        private readonly ICustomReportService<CustomDevExpressReport> _customReportService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customReportService"></param>
        public CustomReportsController(
            ICustomReportService<CustomDevExpressReport> customReportService)
        {
            _customReportService = customReportService;
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
                .GetReportsByTypeId(assetTypeId)
                .Select(r => r.ToModel())
                .ToList();
        }

        /// <summary>
        /// Publish custom report
        /// </summary>
        /// <param name="name">Report name</param>
        /// <param name="fileName">Report template file name</param>
        /// <param name="typeId">Asset type Id</param>
        [Route("publish"), HttpGet]
        public string PublishReport(string name, string fileName, long typeId)
        {
            _customReportService.PublishReport(typeId, name, fileName);
            return string.Format("report [{0}] published", name);
        }

        /// <summary>
        /// Delete custom report 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        [Route("delete"), HttpGet]
        public string DeleteReport(string name, long typeId)
        {
            var result = string.Format("report [{0}] deleted", name);
            try
            {
                _customReportService.DeleteReport(typeId, name);
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            return result;
        }
    }
}