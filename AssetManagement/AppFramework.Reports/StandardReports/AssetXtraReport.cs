
using DevExpress.XtraReports.UI;

namespace AppFramework.Reports
{
    public partial class AssetXtraReport : XtraReport, IReport
    {
        public AssetXtraReport()
        {
            InitializeComponent();
        }

        public ReportType ReportType
        {
            get { return ReportType.AssetReport; }
        }

        public ReportLayout ReportLayout
        {
            get { return ReportLayout.Default; }
        }
    }
}
