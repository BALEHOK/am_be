using DevExpress.XtraReports.Service.Extensions;
using System.Linq;
using DevExpress.XtraReports.UI;
using Common.Logging;
using AppFramework.Reports.Exceptions;
using System;
using AppFramework.Core.AC.Authentication;
using AppFramework.Reports.Properties;

namespace AppFramework.Reports.Services
{
    public class ReportResolver : IReportResolver
    {
        private readonly IStandardReportService _standardReportService;
        private readonly ICustomReportService _customReportService;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
     
        public ReportResolver(
            ICustomReportService customReportService,
            IStandardReportService standardReportService)
        {
            _standardReportService = standardReportService;
            _customReportService = customReportService;
        }

        public XtraReport Resolve(string reportName, bool getParameters)
        {
            _logger.DebugFormat("Resolving a report: {0}", reportName);

            var args = reportName.Split(
                new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (!args.Any())
                throw new InvalidReportParameters(
                    Resources.InvalidParametersGeneral);

            if (args.First() == Constants.ReportTypeCustomReport)
            {
                // ToDo get real user id
                var userId = 1;
                var reportData = _customReportService.GetReportByURI(reportName, userId);
                return _customReportService.CreateReportView(reportData, userId);
            }
            
            if (args.Length != 3)
                throw new InvalidReportParameters(
                    Resources.InvalidParametersForStandardReport);

            ReportType reportType;
            if (!Enum.TryParse(args[1], out reportType))
                throw new InvalidReportParameters(
                    Resources.InvalidStandardReportType);

            LayoutType reportLayout;
            if (!Enum.TryParse(args[2], out reportLayout))
                throw new InvalidReportParameters(
                    Resources.InvalidStandardReportLayout);

            var standardReport = _standardReportService.GetStandardReport(reportType, reportLayout);
            return standardReport;
        }
    }
}
