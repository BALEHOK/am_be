
using DevExpress.XtraReports.UI;

namespace AppFramework.Reports
{
    public partial class AssetsListXtraReport : XtraReport, IReport
    {
        public AssetsListXtraReport()
        {
            InitializeComponent();
        }

        public ReportType ReportType
        {
            get { return ReportType.AssetsListReport; }
        }

        public LayoutType ReportLayout
        {
            get { return LayoutType.Default; }
        }
    }
}
