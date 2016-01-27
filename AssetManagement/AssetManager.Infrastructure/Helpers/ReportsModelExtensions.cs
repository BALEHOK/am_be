using System.Collections.Generic;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models;

namespace AssetManager.Infrastructure.Helpers
{
    public static class ReportsModelExtensions
    {
        public static CustomReportModel ToModel(this Report report, Dictionary<long, string> relatedAssetTypes)
        {
            return new CustomReportModel
            {
                Id = report.ReportUid,
                Name = report.Name,
                AssetTypeName = relatedAssetTypes[report.DynEntityConfigId.GetValueOrDefault()],
                IsFinancial = report.IsFinancial,
                AssetTypeId = report.DynEntityConfigId
            };
        }
    }
}