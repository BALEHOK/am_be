using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Entities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using AppFramework.Core.Classes;
using AppFramework.Core.Exceptions;
using Common.Logging;

namespace AssetManager.Infrastructure.Services
{
    public class ExportService : IExportService
    {
        private readonly IEnvironmentSettings _env;
        private readonly IUserService _userService;
        private readonly IAssetsService _assetsService;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly ILog _logger;

        public ExportService(IEnvironmentSettings envSettings, IAssetTypeRepository assetTypeRepository = null, IAssetsService assetsService = null, ILog logger = null, IUserService userService = null)
        {
            if (envSettings == null)
                throw new ArgumentNullException("envSettings");
            _env = envSettings;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
            if (logger == null)
                throw new ArgumentNullException("logger");
            _logger = logger;
            if (userService == null)
                throw new ArgumentNullException("userService");
            _userService = userService;
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
            dataTable.Columns.Add("DynEntityUid", typeof(string));
            dataTable.Columns.Add("DynEntityId", typeof(string));
            dataTable.Columns.Add("ActiveVersion", typeof(string));
            dataTable.Columns.Add("DynEntityConfigUid", typeof(string));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Revision", typeof(string));
            dataTable.Columns.Add("Barcode", typeof(string));
            dataTable.Columns.Add("Update Date", typeof(string));
            dataTable.Columns.Add("Stock Count", typeof(string));
            dataTable.Columns.Add("Stock Price", typeof(string));
            dataTable.Columns.Add("Document", typeof(string));
            dataTable.Columns.Add("CellenServiceSupport", typeof(string));
            dataTable.Columns.Add("Location", typeof(string));
            dataTable.Columns.Add("Base Location", typeof(string));
            dataTable.Columns.Add("Next Location", typeof(string));
            dataTable.Columns.Add("Department", typeof(string));
            dataTable.Columns.Add("User", typeof(string));
            dataTable.Columns.Add("Owner", typeof(string));
            dataTable.Columns.Add("Update User", typeof(string));


            foreach (var item in searchResults)
            {
                var assetType = _assetTypeRepository.GetById(item.DynEntityConfigId);
                var asset = _assetsService.GetAssetById(item.DynEntityId, assetType);

                var activeVersion = asset.Attributes.Find(attr => attr.Configuration.Name == "ActiveVersion");
                var revision = asset.Attributes.Find(attr => attr.Configuration.Name == "Revision");
                var stockCount = asset.Attributes.Find(attr => attr.Configuration.Name == "Stock Count");
                var stockPrice = asset.Attributes.Find(attr => attr.Configuration.Name == "Stock Price");
                var document = asset.Attributes.Find(attr => attr.Configuration.Name == "Document");
                var cellenServiceSupport = asset.Attributes.Find(attr => attr.Configuration.Name == "CellenServiceSupport");
                var baseLocation = asset.Attributes.Find(attr => attr.Configuration.Name == "Base Location");
                var nextLocation = asset.Attributes.Find(attr => attr.Configuration.Name == "Next Location");
                var user = asset.Attributes.Find(attr => attr.Configuration.Name == "User");
                var owner = asset.Attributes.Find(attr => attr.Configuration.Name == "Owner");
                var updateUser = asset.Attributes.Find(attr => attr.Configuration.Name == "Update User");
                
                dataTable.Rows.Add(
                    item.DynEntityUid,
                    item.DynEntityId,
                    activeVersion != null ? activeVersion.Value: string.Empty,
                    item.DynEntityConfigUid,
                    revision != null ? revision.Value : string.Empty,
                    item.Name,
                    item.BarCode,
                    item.UpdateDate,
                    stockCount != null ? stockCount.Value : string.Empty,
                    stockPrice != null ? stockPrice.Value : string.Empty,
                    document != null ? document.Value : string.Empty,
                    "CellenServiceSupport",
                    item.Location,
                    baseLocation != null ? baseLocation.Value : string.Empty,
                    nextLocation != null ? nextLocation.Value : string.Empty,
                    item.Department,
                    user != null ? user.Value : string.Empty,
                    owner != null ? owner.Value : string.Empty,
                    updateUser != null ? updateUser.Value : string.Empty);
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