
using DevExpress.XtraReports.UI;

namespace AppFramework.Reports
{
    public partial class SearchResultXtraReport : XtraReport, IReport
    {
        public SearchResultXtraReport()
        {
            InitializeComponent();
        }

        public ReportType ReportType
        {
            get { return ReportType.SearchResultReport; }
        }

        public ReportLayout ReportLayout
        {
            get { return ReportLayout.Default; }
        }
    }
}
