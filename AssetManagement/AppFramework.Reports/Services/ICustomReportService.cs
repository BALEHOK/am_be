using AppFramework.Entities;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;

namespace AppFramework.Reports.Services
{
    public interface ICustomReportService
    {
        List<Report> GetAllReports();

        List<Report> GetReportsByAssetTypeId(long assetTypeId);

        Report GetReportById(long reportId);

        Report GetReportByURI(string reportURI);

        void DeleteReport(long reportId);

        Report CreateReport(long assetTypeId, string reportName);

        void UpdateReport(Report reportData, XtraReport report);

        XtraReport CreateReportView(Report reportEntity);
    }
}