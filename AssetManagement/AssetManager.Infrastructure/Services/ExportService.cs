using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Entities;
using AssetManager.Infrastructure;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace AssetManager.Infrastructure.Services
{
    public class ExportService : IExportService
    {
        private readonly IEnvironmentSettings _env;

        public ExportService(IEnvironmentSettings envSettings)
        {
            if (envSettings == null)
                throw new ArgumentNullException("envSettings");
            _env = envSettings;
        }

        public string ExportSearchResultToTxt(IEnumerable<IIndexEntity> searchResults)
        {
            var content = new StringBuilder();
            content.AppendLine("Name,Common Info,Details,Direct Url");
            foreach (IIndexEntity entity in searchResults)
            {
                content.AppendLine(
                    string.Join(",", new string[] { 
                        _env.Escape(entity.Name),
                        _env.Escape(entity.Subtitle), 
                        _env.Escape(entity.DisplayExtValues),
                        _env.GetPathToAssetPage(entity.DynEntityConfigId, entity.DynEntityId)
                    }));
            }
            return content.ToString();
        }

        public string ExportSearchResultToXml(IEnumerable<IIndexEntity> searchResults)
        {
            XNamespace @namespace = "http://tempuri.org/AssetManagementAssets.xsd";
            XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

            document.Add(new XElement(XName.Get("SearchResults", @namespace.NamespaceName),
                from item in searchResults
                select new XElement(XName.Get("SearchResult", @namespace.NamespaceName),
                    new XElement[] { 
                            new XElement(XName.Get("Name", @namespace.NamespaceName), item.Name),
                            new XElement(XName.Get("CommonInfo", @namespace.NamespaceName), item.Subtitle),
                            new XElement(XName.Get("Details", @namespace.NamespaceName), item.DisplayExtValues),
                            new XElement(XName.Get("DirectUrl", @namespace.NamespaceName), 
                                _env.GetPathToAssetPage(item.DynEntityConfigId, item.DynEntityId)),
                        }
                    )));

            return document.ToString();
        }

        public string ExportSearchResultToHtml(IEnumerable<IIndexEntity> searchResults)
        {
            throw new NotImplementedException();
        }

        public byte[] ExportSearchResultToExcel(IEnumerable<IIndexEntity> searchResults)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("CommonInfo", typeof(string));
            dataTable.Columns.Add("Details", typeof(string));
            dataTable.Columns.Add("DirectUrl", typeof(string));

            foreach (var item in searchResults)
            {
                dataTable.Rows.Add(
                    item.Name,
                    item.Subtitle,
                    item.DisplayExtValues,
                    _env.GetPathToAssetPage(item.DynEntityConfigId, item.DynEntityId));
            }

            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Export");

                //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                ws.Cells["A1"].LoadFromDataTable(dataTable, true);

                //Format the header
                using (ExcelRange rng = ws.Cells["A1:D1"])
                {
                    rng.Style.Font.Bold = true;
                }

                return pck.GetAsByteArray();
            }
        }

        private DataTable AssetsToDataTable(IEnumerable<AppFramework.Core.Classes.Asset> assets)
        {
            DataTable dt = new DataTable("Sheet1$");
            dt.Columns.Add(AttributeNames.DynEntityId);
            bool isInitialized = false;

            foreach (var asset in assets)
            {
                //Init columns
                if (!isInitialized)
                {
                    foreach (var attribute in asset.Attributes.Where(x => x.GetConfiguration().IsShownOnPanel))
                    {
                        var columnName = attribute.GetConfiguration().Name;
                        dt.Columns.Add(columnName);
                    }
                    isInitialized = true;
                }

                var dr = dt.NewRow();
                dr[AttributeNames.DynEntityId] = asset.ID;
                foreach (var attribute in asset.Attributes.Where(x => x.GetConfiguration().IsShownOnPanel))
                {
                    var columnName = attribute.GetConfiguration().Name;
                    dr[columnName] = attribute.Value;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}