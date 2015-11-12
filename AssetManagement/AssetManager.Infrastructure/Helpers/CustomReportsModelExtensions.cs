using System.IO;
using AppFramework.Reports.CustomReports;
using AssetManager.Infrastructure.Models;

namespace AssetManager.Infrastructure.Helpers
{
    public static class CustomReportsModelExtensions
    {
        public static CustomReportModel ToModel(this CustomDevExpressReport report)
        {
            return new CustomReportModel
            {
                Id = report.Id,
                Name = report.Name,
                AssetTypeId = report.AssetTypeId,
                AssetTypeName = report.AssetTypeName,
                IsFinancial = report.IsFinancial,
                FileName = Path.GetFileName(report.FileName)
            };
        }
    }
}