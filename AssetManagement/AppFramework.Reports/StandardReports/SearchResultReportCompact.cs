
using DevExpress.XtraReports.UI;

namespace AppFramework.Reports
{
    public partial class SearchResultXtraReportCompact : XtraReport, IReport
    {
        public SearchResultXtraReportCompact()
        {
            InitializeComponent();
        }

        public ReportType ReportType
        {
            get { return ReportType.SearchResultReport; }
        }

        public LayoutType ReportLayout
        {
            get { return LayoutType.Compact; }
        }
    }
}
