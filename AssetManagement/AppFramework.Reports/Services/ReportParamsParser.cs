using AppFramework.Reports.Exceptions;
using AppFramework.Reports.Properties;
using System.Linq;

namespace AppFramework.Reports.Services
{
    class ReportParamsParser
    {
        public static CustomReportParams GetCustomReportParams(string[] args)
        {
            if (args.Count() < 2)
                throw new InvalidReportParameters(
                    Resources.InvalidParametersForCustomReport);

            var result = new CustomReportParams
            {
                AssetTypeId = long.Parse(args[1])
            };

            if (args.Count() == 3)
                result.ReportId = long.Parse(args[2]);

            return result;
        }
    }
}
