using AppFramework.Entities;
using AppFramework.Reports.Services;
using AssetManagerAdmin.Infrastructure.Messages;
using DevExpress.Xpf.Reports.UserDesigner;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.XtraReports.Service.Extensions;
using DevExpress.XtraReports.UI;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading;
using AppFramework.Core.AC.Authentication;

namespace AssetManagerAdmin.Services
{
    class DbReportStorage : IReportStorage
    {
        private static readonly AutoResetEvent DialogEvent = new AutoResetEvent(false);
        private readonly IReportResolver _reportResolver;
        private readonly ICustomReportService _reportService;

        public DbReportStorage(IReportResolver reportResolver, ICustomReportService reportService)
        {
            _reportResolver = reportResolver;
            _reportService = reportService;
        }

        public bool CanCreateNew()
        {
            return true;
        }

        public bool CanOpen()
        {
            return true;
        }

        public XtraReport CreateNew()
        {
            return new XtraReport();
        }

        public XtraReport CreateNewSubreport()
        {
            throw new NotImplementedException();
        }

        public string GetErrorMessage(Exception exception)
        {
            return ExceptionHelper.GetInnerErrorMessage(exception);
        }

        public XtraReport Load(string reportId, IReportSerializer designerReportSerializer)
        {
            if (reportId == string.Empty)
                return new XtraReport();
            return _reportResolver.Resolve(reportId, false);
        }

        public string Open(IReportDesignerUI designer)
        {
            OpenReportDialogCallbackMessage dialogResult = null;
            var msg = new OpenReportMessage(this, r =>
            {
                dialogResult = r;
                DialogEvent.Set();
            });

            DialogEvent.Reset();

            Messenger.Default.Send(msg);

            DialogEvent.WaitOne();

            if (dialogResult != null && dialogResult.Result == true)
                return string.Format("{0}/{1}/{2}",
                    AppFramework.Reports.Constants.ReportTypeCustomReport,
                    dialogResult.Report.DynEntityConfigId,
                    dialogResult.Report.ReportUid);

            return null;
        }

        public string Save(string reportId, IReportProvider reportProvider, bool saveAs, string reportTitle, IReportDesignerUI designer)
        {
            Report reportEntity = null;

            // ToDo get real user id
            var userId = 1;

            if (reportId != null)
            {
                reportEntity = _reportService.GetReportByURI(reportId, userId);
            }

            if (saveAs || reportEntity == null)
            {
                SaveReportDialogCallbackMessage dialogResult = null;
                var msg = new SaveReportMessage(this, r =>
                {
                    dialogResult = r;
                    DialogEvent.Set();
                });

                DialogEvent.Reset();

                Messenger.Default.Send(msg);

                DialogEvent.WaitOne();

                if (dialogResult == null || dialogResult.Result != true)
                    return null;

                reportEntity = _reportService.CreateReport(
                    dialogResult.AssetTypeId, dialogResult.ReportName, userId);
            }

            var report = reportProvider.GetReport();
            _reportService.UpdateReport(reportEntity, report, userId);

            return string.Format("{0}/{1}/{2}",
                AppFramework.Reports.Constants.ReportTypeCustomReport,
                reportEntity.DynEntityConfigId,
                reportEntity.ReportUid);
        }
    }
}
