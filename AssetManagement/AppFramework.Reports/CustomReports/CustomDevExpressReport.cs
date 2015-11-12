using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.UI;

namespace AppFramework.Reports.CustomReports
{
    public class CustomDevExpressReport : ICustomReport<XtraReport>
    {
        private XtraReport _report;
        private readonly string _name;
        private readonly string _fileName;        
        private readonly long _assetTypeId;
        private readonly string _assetTypeName;
        private readonly long _reportId;
        private string _filterString;
        private readonly bool _isFinancial;
        private readonly string _connectionString;

        public CustomDevExpressReport(long reportId, long assetTypeId, string name, string fileName, string connectionString)
        {
            _reportId = reportId;
            _assetTypeId = assetTypeId;
            _name = name;
            _fileName = fileName;
            _connectionString = connectionString;
        }

        public CustomDevExpressReport(long reportId, long assetTypeId, string assetTypeName, string name, string fileName, bool isFinancial, string connectionString)
        {
            _reportId = reportId;
            _assetTypeId = assetTypeId;
            _assetTypeName = assetTypeName;
            _name = name;
            _fileName = fileName;
            _isFinancial = isFinancial;
            _connectionString = connectionString;
        }

        #region ICustomReport

        public long Id
        {
            get { return _reportId; }
        }

        public long AssetTypeId
        {
            get { return _assetTypeId; }
        }

        public string AssetTypeName
        {
            get { return _assetTypeName; }
        }

        public XtraReport ReportObject
        {
            get { return _report ?? (_report = LoadReport()); }
        }

        public string Name
        {
            get { return _name; }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public bool IsFinancial
        {
            get { return _isFinancial; }
        }

        #endregion

        private XtraReport LoadReport()
        {
            var report = new XtraReport();
            report.LoadLayout(_fileName);

            if (!string.IsNullOrEmpty(_connectionString))
            {
                var dataSource = ((SqlDataSource) report.DataSource);
                dataSource.Connection.ConnectionString = _connectionString;                
            }

            report.FilterString = _filterString;
            report.CreateDocument(false);

            return report;
        }

        public string Filter
        {
            set
            {
                _filterString = value;
                if (_report != null)
                {
                    _report.Dispose();
                    _report = null;
                }                
            }
        }
    }
}