using AppFramework.Entities;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;

namespace AppFramework.Reports.Services
{
    public interface ICustomReportService
    {
        List<Report> GetAllReports(long userId);

        List<Report> GetReportsByAssetTypeId(long assetTypeId, long userId);

        Report GetReportById(long reportId, long userId);

        Report GetReportByURI(string reportUri, long userId);

        void DeleteReport(long reportId, long userId);

        Report CreateReport(long assetTypeId, string reportName, long userId);

        void UpdateReport(Report reportData, XtraReport report, long userId);

        XtraReport CreateReportView(Report reportEntity, long userId);
    }
}