using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppFramework.Reports.Models;

namespace AppFramework.Reports.StandardReports.DataProviders
{
    public class SearchResultReportDataProvider : IReportDataProvider<SearchResultXtraReport>
    {
        public AssetsContainer GetData(long assetTypeId, long assetUid, long currentUserId)
        {
            throw new NotImplementedException();
        }
    }
}
