using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Entities;
using OfficeOpenXml;

namespace AssetManager.Infrastructure.Services
{
    public class ExportSearchResultService : IExportService
    {
        private readonly IEnvironmentSettings _env;
        private readonly ISearchService _searchService;

        public ExportSearchResultService(IEnvironmentSettings envSettings, ISearchService searchService)
        {
            if (envSettings == null)
                throw new ArgumentNullException("envSettings");
            _env = envSettings;

            if (searchService == null)
                throw new ArgumentNullException("searchService");
            _searchService = searchService;
        }

        public byte[] ExportToXml(SearchTracking searchTracking, long userId)
        {
            var searchResults = _searchService.GetSearchResultsByTracking(searchTracking, userId);

            XNamespace @namespace = "http://tempuri.org/AssetManagementAssets.xsd";
            var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

            document.Add(new XElement(XName.Get("SearchResults", @namespace.NamespaceName),
                from item in searchResults
                select
                    new XElement(XName.Get("SearchResult", @namespace.NamespaceName),
                        new XElement(XName.Get("Name", @namespace.NamespaceName), item.Name),
                        new XElement(XName.Get("CommonInfo", @namespace.NamespaceName), item.Subtitle),
                        new XElement(XName.Get("Details", @namespace.NamespaceName), item.DisplayExtValues),
                        new XElement(XName.Get("DirectUrl", @namespace.NamespaceName),
                            _env.GetPathToAssetPage(item.DynEntityConfigId, item.DynEntityId)))));

            return Encoding.UTF8.GetBytes(document.ToString());
        }

        public byte[] ExportToExcel(SearchTracking searchTracking, long userId)
        {
            var searchResults = _searchService.GetSearchResultsByTracking(searchTracking, userId);

            var dataTable = new DataTable();
            dataTable.Columns.Add("Name", typeof (string));
            dataTable.Columns.Add("CommonInfo", typeof (string));
            dataTable.Columns.Add("Details", typeof (string));
            dataTable.Columns.Add("DirectUrl", typeof (string));

            foreach (var item in searchResults)
            {
                dataTable.Rows.Add(
                    item.Name,
                    item.Subtitle,
                    item.DisplayExtValues,
                    _env.GetPathToAssetPage(item.DynEntityConfigId, item.DynEntityId));
            }

            using (var pck = new ExcelPackage())
            {
                //Create the worksheet
                var ws = pck.Workbook.Worksheets.Add("Export");

                //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                ws.Cells["A1"].LoadFromDataTable(dataTable, true);

                //Format the header
                using (var rng = ws.Cells["A1:D1"])
                {
                    rng.Style.Font.Bold = true;
                }

                return pck.GetAsByteArray();
            }
        }

        public byte[] ExportToTxt(SearchTracking searchTracking, long userId)
        {
            var searchResults = _searchService.GetSearchResultsByTracking(searchTracking, userId);

            var content = new StringBuilder();
            content.AppendLine("Name,Common Info,Details,Direct Url");
            foreach (var entity in searchResults)
            {
                content.AppendLine(
                    string.Join(",", _env.Escape(entity.Name), _env.Escape(entity.Subtitle),
                        _env.Escape(entity.DisplayExtValues),
                        _env.GetPathToAssetPage(entity.DynEntityConfigId, entity.DynEntityId)));
            }
            return Encoding.UTF8.GetBytes(content.ToString());
        }
    }
}