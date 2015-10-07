using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.PL
{
    public class RightsEntryGridRow
    {
        public long ViewID { get; set; }
        public string Categories { get; set; }
        public string AssetTypes { get; set; }
        public string Departments { get; set; }
        public string Permission { get; set; }
        public bool IsDeny { get; set; }
        public bool CanDelete { get; set; }
    }
}
