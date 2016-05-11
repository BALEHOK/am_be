using System.Collections.Generic;
using System.Linq;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AppFramework.Reports.Exceptions;
using DevExpress.XtraReports.UI;
using System.IO;
using DevExpress.DataAccess.Sql;
using System;
using AppFramework.Reports.Permissions;
using AppFramework.Reports.Properties;

namespace AppFramework.Reports.Services
{
    public class DevExpressCustomReportsService : ICustomReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportPermissionChecker _reportPermissionChecker;
        private readonly string _connectionString;

        public DevExpressCustomReportsService(IUnitOfWork unitOfWork, IReportPermissionChecker reportPermissionChecker)
        {
            _unitOfWork = unitOfWork;
            _reportPermissionChecker = reportPermissionChecker;
            _connectionString = _unitOfWork.SqlProvider.ConnectionString;
        }

        public List<Report> GetAllReports(long userId)
        {
            var leftOuterJoin = from r in _unitOfWork.ReportRepository.AsQueryable()
                                join at in _unitOfWork.DynEntityConfigRepository
                                                      .AsQueryable()
                                                      .Where(at => at.ActiveVersion)
                                on r.DynEntityConfigId equals at.DynEntityConfigId into atLeftJoin
                                from at in atLeftJoin.DefaultIfEmpty()
                                select new { r, at };

            var reports = leftOuterJoin
                .ToList()
                .Select(x =>
                    new Report
                    {
                        ReportUid = x.r.ReportUid,
                        Name = x.r.Name,
                        Type = x.r.Type,
                        LayoutData = x.r.LayoutData,
                        IsFinancial = x.r.IsFinancial,
                        DynEntityConfigId = x.r.DynEntityConfigId,
                        AssetTypeName = x.at == null
                            ? string.Empty
                            : x.at.Name
                    });

            return _reportPermissionChecker
                .FilterReadPermitted(reports, userId)
                .ToList();
        }

        public List<Report> GetReportsByAssetTypeId(long assetTypeId, long userId)
        {
            return _reportPermissionChecker.FilterReadPermitted(
                    _unitOfWork.ReportRepository.Where(r => 
                        r.Type == (int)ReportType.CustomReport && 
                        r.DynEntityConfigId == assetTypeId),
                    userId)
                .ToList();
        }

        public Report GetReportByURI(string reportUri, long userId)
        {
            var args = reportUri.Split(
                new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (!args.Any())
                throw new InvalidReportParameters(
                    Resources.InvalidParametersGeneral);

            if (args.First() != Constants.ReportTypeCustomReport)
                throw new NotSupportedException(Resources.InvalidParametersForCustomReport);

            var reportParams = ReportParamsParser.GetCustomReportParams(args);
            return GetReportById(reportParams.ReportId, userId);
        }

        public Report GetReportById(long reportId, long userId)
        {
            var report = _unitOfWork.ReportRepository
                .SingleOrDefault(r => r.ReportUid == reportId);

            if (report == null)
                throw new ReportNotFound();

            if (!_reportPermissionChecker.HasReadPermission(report, userId))
            {
                throw new AccessViolationException(string.Format("User {0} has no read permission on the report {1}", userId, reportId));
            }

            return report;
        }
                
        public void DeleteReport(long reportId, long userId)
        {
            var report = _unitOfWork.ReportRepository.Single(
                r => r.ReportUid == reportId);

            if (!_reportPermissionChecker.HasDeletePermission(report, userId))
            {
                throw new AccessViolationException(string.Format("User {0} has no delete permission on the report {1}", userId, reportId));
            }

            _unitOfWork.ReportRepository.Delete(report);
        }

        public Report CreateReport(long assetTypeId, string reportName, long userId)
        {
            var report = new Report
            {
                DynEntityConfigId = assetTypeId,
                Type = (int)ReportType.CustomReport,
                Name = reportName
            };

            EnsureEditPermission(report, userId);

            _unitOfWork.ReportRepository.Insert(report);
            _unitOfWork.Commit();

            return report;
        }

        public void UpdateReport(Report reportData, XtraReport report, long userId)
        {
            EnsureEditPermission(reportData, userId);

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

        public XtraReport CreateReportView(Report reportEntity, long userId)
        {
            EnsureEditPermission(reportEntity, userId);

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

        private void EnsureEditPermission(Report report, long userId)
        {
            if (!_reportPermissionChecker.HasEditPermission(report, userId))
            {
                throw new AccessViolationException(string.Format("User {0} has no edit permission on the report {1}", userId, report.ReportUid));
            }
        }
    }
}