using System.Collections.Generic;
using AppFramework.Entities;

namespace AppFramework.Reports.Permissions
{
    public interface IReportPermissionChecker
    {
        IEnumerable<Report> FilterReadPermitted(IEnumerable<Report> reports, long userId);
        bool HasEditPermission(Report report, long userId);
        bool HasReadPermission(Report report, long userId);
        bool HasDeletePermission(Report report, long userId);
    }
}