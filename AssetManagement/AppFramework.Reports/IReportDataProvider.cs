using AppFramework.Reports.Models;
using DevExpress.XtraReports.UI;
using System;

namespace AppFramework.Reports
{
    public interface IReportDataProvider<T>
        where T : XtraReport
    {
        AssetsContainer GetData(long assetTypeId, long assetId, long currentUserId);
    }
}
