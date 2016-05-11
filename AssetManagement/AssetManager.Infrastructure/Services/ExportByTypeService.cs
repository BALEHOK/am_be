using System;
using System.Data;
using System.IO;
using System.Text;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.IE;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Entities;
using OfficeOpenXml;

namespace AssetManager.Infrastructure.Services
{
    public class ExportByTypeService : IExportService
    {
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetsExporter _assetsExporter;

        public ExportByTypeService(IAssetTypeRepository assetTypeRepository,
            IAssetsExporter assetsExporter)
        {
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;

            if (assetsExporter == null)
                throw new ArgumentNullException("assetsExporter");
            _assetsExporter = assetsExporter;
        }

        public byte[] ExportToXml(SearchTracking searchTracking, long userId)
        {
            var dataTable = GetAssetRecords(searchTracking, userId);

            var outputStream = new MemoryStream();

            dataTable.WriteXml(outputStream);

            return outputStream.ToArray();
        }

        public byte[] ExportToExcel(SearchTracking searchTracking, long userId)
        {
            var dataTable = GetAssetRecords(searchTracking, userId);

            using (var pck = new ExcelPackage())
            {
                //Create the worksheet
                var ws = pck.Workbook.Worksheets.Add("Export");

                //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                ws.Cells["A1"].LoadFromDataTable(dataTable, true);

                //Format the header
                ws.Row(1).Style.Font.Bold = true;

                return pck.GetAsByteArray();
            }
        }

        public byte[] ExportToTxt(SearchTracking searchTracking, long userId)
        {
            var dataTable = GetAssetRecords(searchTracking, userId);

            var content = new StringBuilder();
            content.AppendLine("Name,Common Info,Details,Direct Url");
            foreach (DataRow row in dataTable.Rows)
            {
                content.AppendLine(string.Join(",", row.ItemArray));
            }
            return Encoding.UTF8.GetBytes(content.ToString());
        }

        private DataTable GetAssetRecords(SearchTracking searchTracking, long userId)
        {
            var parameters = SearchParameters.GetFromXml(searchTracking.Parameters);
            var assetTypeId = long.Parse(parameters.ConfigsIds);
            var type = _assetTypeRepository.GetById(assetTypeId);

            var dataTable = _assetsExporter.ExportToDataTable(type.UID, parameters.Elements, userId);
            dataTable.TableName = type.Name;
            return dataTable;
        }
    }
}