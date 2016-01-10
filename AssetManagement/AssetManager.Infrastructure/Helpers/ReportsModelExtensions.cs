using AppFramework.Entities;
using AssetManager.Infrastructure.Models;

namespace AssetManager.Infrastructure.Helpers
{
    public static class ReportsModelExtensions
    {
        public static CustomReportModel ToModel(this Report report)
        {
            return new CustomReportModel
            {
                Id = report.ReportUid,
                Name = report.Name,
                AssetTypeName = report.DynEntityConfigId.ToString(), // TODO
                IsFinancial = report.IsFinancial,
                AssetTypeId = report.DynEntityConfigId
            };
        }
    }
}