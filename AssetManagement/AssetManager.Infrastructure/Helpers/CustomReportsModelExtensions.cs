using AppFramework.Reports.CustomReports;
using AssetManager.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Infrastructure.Helpers
{
    public static class CustomReportsModelExtensions
    {
        public static CustomReportModel ToModel(this CustomDevExpressReport report)
        {
            return new CustomReportModel
            {
                Id = report.Id,
                AssetTypeId = report.AssetTypeId,
                FileName = Path.GetFileName(report.FileName),
                Name = report.Name
            };
        }
    }
}
