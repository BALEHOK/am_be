using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace AppFramework.Reports
{
    public partial class AssetsWithChildsReport : DevExpress.XtraReports.UI.XtraReport, IReport
    {
        public AssetsWithChildsReport()
        {
            InitializeComponent();
        }

        public ReportType ReportType
        {
            get { return ReportType.AssetsWithChildsReport; }
        }

        public LayoutType ReportLayout
        {
            get { return LayoutType.Default; }
        }
    }
}
