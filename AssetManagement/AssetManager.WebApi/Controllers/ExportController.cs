using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Exceptions;
using AssetManager.Infrastructure.Services;

namespace AssetManager.WebApi.Controllers
{
    // TODO: add security
    [AllowAnonymous]
    public class ExportController : Controller
    {
        private readonly ISearchTracker _searchTracker;
        private readonly IExportService _exportService;

        public ExportController(ISearchTracker searchTracker, IExportService exportService)
        {
            if (searchTracker == null)
                throw new ArgumentNullException("searchTracker");
            _searchTracker = searchTracker;

            if (exportService == null)
                throw new ArgumentNullException("exportService");
            _exportService = exportService;
        }

        /// <summary>
        /// Exports search results to a file
        /// </summary>
        /// <param name="searchId">Search Id</param>
        /// <returns></returns>
        public ActionResult Index(Guid searchId, string format)
        {
            var formatLower = format.ToLower();

            //TODO: get actual current user
            var userId = 1L;
            var searchTracking = _searchTracker.GetTrackingBySearchIdUserId(searchId, userId);
            if (searchTracking == null)
                throw new EntityNotFoundException("Cannot find search request parameters by given SearchId");

            byte[] exportContent;
            string contentType;
            switch (formatLower)
            {
                case "xml":
                    exportContent = _exportService.ExportToXml(searchTracking, userId);
                    contentType = "text/xml";
                    break;

                case "xlsx":
                    exportContent = _exportService.ExportToExcel(searchTracking, userId);
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;

                case "txt":
                    exportContent = _exportService.ExportToTxt(searchTracking, userId);
                    contentType = "text/plain";
                    break;

                default:
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Unsupported format to export");
            }

            return new FileContentResult(exportContent, contentType)
                {
                    FileDownloadName = string.Format("search-export-{0}.{1}",
                        Routines.SanitizeFileName(DateTime.Now.ToShortDateString()),
                        format)
                };
        }
    }
}