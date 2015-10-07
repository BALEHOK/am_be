using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Reports
{
    public interface IReport
    {
        ReportType ReportType { get; }
        ReportLayout ReportLayout { get; }
    }
}
