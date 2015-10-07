using System.Collections.Generic;

namespace AppFramework.Reports.Services
{
    public interface ICustomReportService<T>
    {
        List<T> GetAllReports();
        List<T> GetReportsByTypeId(long assetTypeId);
        T GetReportById(long reportId);
        void PublishReport(long assetTypeId, string reportName, string fileName);
        void DeleteReport(long assetTypeId, string reportName);
    }
}