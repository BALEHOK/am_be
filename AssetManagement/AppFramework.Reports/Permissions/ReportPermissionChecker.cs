using System.Collections.Generic;
using System.Linq;
using AppFramework.Core.AC.Authentication;
using AppFramework.DataProxy;
using AppFramework.Entities;

namespace AppFramework.Reports.Permissions
{
    public class ReportPermissionChecker : IReportPermissionChecker
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportPermissionChecker(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Report> FilterReadPermitted(IEnumerable<Report> reports, long userId)
        {
            var permittedFinancialAssetTypes = _unitOfWork.GetPermittedAssetTypes(userId, (byte)Permission.DDRD).ToList();
            var permittedAssetTypes = _unitOfWork.GetPermittedAssetTypes(userId, (byte)Permission.RDDD).ToList();
            return reports.Where(r => r.IsFinancial
                ? permittedFinancialAssetTypes.Any(id => id == r.DynEntityConfigId)
                : permittedAssetTypes.Any(id => id == r.DynEntityConfigId));
        }

        public bool HasReadPermission(Report report, long userId)
        {
            return HasPermission(report, userId, report.IsFinancial ? Permission.DDRD : Permission.RDDD);
        }

        public bool HasEditPermission(Report report, long userId)
        {
            return HasPermission(report, userId, report.IsFinancial ? Permission.DDDW : Permission.DWDD);
        }

        public bool HasDeletePermission(Report report, long userId)
        {
            return HasPermission(report, userId, Permission.ReadWriteDelete);
        }

        private bool HasPermission(Report report, long userId, Permission permission)
        {
            return
                _unitOfWork.GetPermittedAssetTypes(userId, (byte)permission)
                    .Contains(report.DynEntityConfigId.GetValueOrDefault());
        }
    }
}