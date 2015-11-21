using AppFramework.Core.Classes;
using AppFramework.Core.Classes.SearchEngine;
using AssetManager.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AssetManager.WebApi.Controllers
{
    // TODO: add security
    [AllowAnonymous]
    public class ExportController : Controller
    {
        private readonly ISearchService _searchService;
        private readonly IExportService _exportService;

        private static string[] AllowedFormats = new string[] { "xml", "txt", "xlsx" };

        public ExportController(
            ISearchService searchService,
            IExportService exportService)
        {
            if (searchService == null)
                throw new ArgumentNullException("searchService");
            _searchService = searchService;
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
            if (!AllowedFormats.Any(f => f == formatLower))
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest,
                    "Unsupported format to export");

            //TODO: get actual current user
            long userId = 1;
            var searchResults = _searchService.GetSearchResultsBySearchId(searchId, userId);
            byte[] exportContent;
            string contentType;

            if (formatLower == "xml")
            {
                exportContent = Encoding.UTF8.GetBytes(
                    _exportService.ExportSearchResultToXml(searchResults));
                contentType = "text/xml";
            }
            else if (formatLower == "xlsx")
            {
                exportContent = _exportService.ExportSearchResultToExcel(searchResults);
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
            else
            {
                exportContent = Encoding.UTF8.GetBytes(
                    _exportService.ExportSearchResultToTxt(searchResults));
                contentType = "text/plain";
            }

            Response.AddHeader("content-disposition", "attachment; filename=" +
                string.Format("search-export-{0}.{1}",
                    Routines.SanitizeFileName(DateTime.Now.ToShortDateString()),
                    format));

            return new FileContentResult(exportContent, contentType);
        }
    }
}