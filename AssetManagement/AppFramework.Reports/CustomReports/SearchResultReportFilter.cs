using System;
using System.Data.SqlClient;
using System.Linq;
using AppFramework.DataProxy;
using Dapper;

namespace AppFramework.Reports.CustomReports
{
    public class SearchResultReportFilter
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchResultReportFilter(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string GetFilterString(Guid searchId)
        {
            var queryParameters = new DynamicParameters();
            queryParameters.Add("SearchId", searchId);
            const string queryString = @"SELECT [DynEntityUid] FROM [IndexActiveDynEntities] 
                                         WHERE IndexUid in (
                                                            SELECT IndexUid FROM [_search_srchres]
                                                            WHERE SearchId = @SearchId
                                                           )";

            using (var connection = new SqlConnection(_unitOfWork.SqlProvider.ConnectionString))
            {
                var queryResult = connection.Query<string>(queryString, queryParameters).ToList();

                var idList = queryResult.Count > 0 ? string.Join(",", queryResult) : "null";
                var filterString = string.Format("uid in ({0})", idList);

                return filterString;
            }
        }
    }
}