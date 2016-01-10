using System.Collections.Generic;
using System.Linq;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AppFramework.Reports.Exceptions;
using DevExpress.XtraReports.UI;
using System.IO;
using DevExpress.DataAccess.Sql;
using System;
using AppFramework.Reports.Properties;

namespace AppFramework.Reports.Services
{
    public class DevExpressCustomReportsService : ICustomReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _connectionString;

        public DevExpressCustomReportsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _connectionString = _unitOfWork.SqlProvider.ConnectionString;
        }

        public List<Report> GetAllReports()
        {
            var reports = _unitOfWork.ReportRepository.Get();
            return reports.ToList();
        }

        public List<Report> GetReportsByAssetTypeId(long assetTypeId)
        {
            return _unitOfWork.ReportRepository.Where(r => 
                    r.Type == (int)ReportType.CustomReport && 
                    r.DynEntityConfigId == assetTypeId)
                .ToList();
        }

        public Report GetReportByURI(string reportURI)
        {
            var args = reportURI.Split(
                new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (!args.Any())
                throw new InvalidReportParameters(
                    Resources.InvalidParametersGeneral);

            if (args.First() != Constants.ReportTypeCustomReport)
                throw new NotSupportedException(Resources.InvalidParametersForCustomReport);

            var reportParams = ReportParamsParser.GetCustomReportParams(args);
            return GetReportById(reportParams.ReportId);
        }

        public Report GetReportById(long reportId)
        {
            var reportEntity = _unitOfWork.ReportRepository
                .SingleOrDefault(r => r.ReportUid == reportId);

            if (reportEntity == null)
                throw new ReportNotFound();
            return reportEntity;
        }
                
        public void DeleteReport(long reportId)
        {
            var report = _unitOfWork.ReportRepository.Single(
                r => r.ReportUid == reportId);
            _unitOfWork.ReportRepository.Delete(report);
        }

        public Report CreateReport(long assetTypeId, string reportName)
        {
            var report = new Report
            {
                DynEntityConfigId = assetTypeId,
                Type = (int)ReportType.CustomReport,
                Name = reportName
            };

            _unitOfWork.ReportRepository.Insert(report);
            _unitOfWork.Commit();

            return report;
        }

        public void UpdateReport(Report reportData, XtraReport report)
        {
            if (report != null)
            {
                using (var stream = new MemoryStream())
                {
                    report.SaveLayout(stream);
                    reportData.LayoutData = stream.ToArray();
                }
            }
            _unitOfWork.ReportRepository.Update(reportData);
            _unitOfWork.Commit();
        }

        public XtraReport CreateReportView(Report reportEntity)
        {
            var report = new XtraReport();
            
            if (reportEntity.LayoutData != null)
            {
                using (var stream = new MemoryStream(reportEntity.LayoutData))
                {
                    report = XtraReport.FromStream(stream, true);
                }
            }

            if (!string.IsNullOrEmpty(_connectionString) && report.DataSource != null)
            {
                var dataSource = ((SqlDataSource)report.DataSource);
                dataSource.Connection.ConnectionString = _connectionString;
            }

            report.DisplayName = reportEntity.Name;

            return report;
        }

    }
}