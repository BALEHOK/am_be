
using System.Collections.Generic;

namespace AppFramework.Reports.Models
{
    public class AssetsContainer
    {
        public string ReportTitle { get; set; }
        public string ReportSubtitle { get; set; }

        public List<AssetViewModel> Assets { get; set; }

        public AssetsContainer()
        {
            Assets = new List<AssetViewModel>();
        }
    }
}
