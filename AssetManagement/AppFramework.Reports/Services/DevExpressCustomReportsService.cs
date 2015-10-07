using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AppFramework.Reports.CustomReports;
using DevExpress.XtraReports.UI;

namespace AppFramework.Reports.Services
{
    public class DevExpressCustomReportsService : ICustomReportService<CustomDevExpressReport>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _connectionString;

        public DevExpressCustomReportsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _connectionString = _unitOfWork.SqlProvider.ConnectionString;
        }

        public List<CustomDevExpressReport> GetAllReports()
        {
            var reports = _unitOfWork.ReportRepository.Where(r => r.DynConfigId != null && r.Type == 10).ToList();
            var result =
                reports.Select(
                    r =>
                        // ReSharper disable once PossibleInvalidOperationException
                        new CustomDevExpressReport(r.ReportUid, (long)r.DynConfigId, r.Name,
                            GetReportFilePath(r.ReportFile), _connectionString));

            return result.ToList();
        }

        public List<CustomDevExpressReport> GetReportsByTypeId(long assetTypeId)
        {
            var reports = _unitOfWork.ReportRepository.Where(r => r.Type == 10 && r.DynConfigId == assetTypeId).ToList();

            var result =
                reports.Select(
                    r => new CustomDevExpressReport(r.ReportUid, assetTypeId, r.Name, GetReportFilePath(r.ReportFile), _connectionString));

            return result.ToList();
        }

        public CustomDevExpressReport GetReportById(long reportId)
        {
            var report =
                _unitOfWork.ReportRepository.Where(r => r.ReportUid == reportId && r.DynConfigId != null)
                    .Select(
                        r =>
                            // ReSharper disable once PossibleInvalidOperationException
                            new CustomDevExpressReport(r.ReportUid, (long)r.DynConfigId, r.Name,
                                GetReportFilePath(r.ReportFile), _connectionString))
                    .SingleOrDefault();

            return report;
        }

        public void PublishReport(long assetTypeId, string reportName, string fileName)
        {
            var report = new Report
            {
                DynConfigId = assetTypeId,
                Name = reportName,
                Type = 10,
                ReportFile = fileName
            };

            _unitOfWork.ReportRepository.Insert(report);
            _unitOfWork.Commit();
        }

        public void DeleteReport(long assetTypeId, string reportName)
        {
            var report = _unitOfWork.ReportRepository.Single(
                r => r.Type == 10 && r.DynConfigId == assetTypeId && r.Name.Equals(reportName));
            var path = GetReportFilePath(report.ReportFile);

            _unitOfWork.ReportRepository.Delete(report);
            File.Delete(path);
        }
        
        private static string GetReportFilePath(string fileName)
        {
            //todo: path should be the same after old UI will be completely removed
            var uploadDir = ConfigurationManager.AppSettings["ReportTemplatesUploadDir"];
            if (uploadDir.StartsWith("~"))
                uploadDir = HostingEnvironment.MapPath(uploadDir);

            var reportFilePath = Path.Combine(uploadDir, fileName);            
            return reportFilePath;
        }
    }
}