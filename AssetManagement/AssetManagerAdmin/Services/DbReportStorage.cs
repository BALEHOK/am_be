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

namespace AssetManagerAdmin.Services
{
    class DbReportStorage : IReportStorage
    {
        private static AutoResetEvent dialogEvent = new AutoResetEvent(false);
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
            SaveReportDialogCallbackMessage dialogResult = null;
            var msg = new SaveReportMessage(this, (r) =>
            {
                dialogResult = r;
                dialogEvent.Set();
            });

            dialogEvent.Reset();

            Messenger.Default.Send(msg);

            dialogEvent.WaitOne();

            if (dialogResult == null || dialogResult.Result != true)
                return null;

            var reportEntity = _reportService.CreateReport(
                dialogResult.AssetTypeId, dialogResult.ReportName);

            return _reportService.CreateReportView(reportEntity);
        }

        public XtraReport CreateNewSubreport()
        {
            throw new NotImplementedException();
        }

        public string GetErrorMessage(Exception exception)
        {
            return ExceptionHelper.GetInnerErrorMessage(exception);
        }

        public XtraReport Load(string reportID, IReportSerializer designerReportSerializer)
        {
            if (reportID == string.Empty)
                return new XtraReport();
            return _reportResolver.Resolve(reportID, false);
        }

        public string Open(IReportDesignerUI designer)
        {
            OpenReportDialogCallbackMessage dialogResult = null;
            var msg = new OpenReportMessage(this, (r) =>
            {
                dialogResult = r;
                dialogEvent.Set();
            });

            dialogEvent.Reset();

            Messenger.Default.Send(msg);

            dialogEvent.WaitOne();

            if (dialogResult != null && dialogResult.Result == true)
                return string.Format("{0}/{1}/{2}",
                    AppFramework.Reports.Constants.ReportTypeCustomReport,
                    dialogResult.Report.DynEntityConfigId,
                    dialogResult.Report.ReportUid);
            else
                return string.Empty;
        }

        public string Save(string reportID, IReportProvider reportProvider, bool saveAs, string reportTitle, IReportDesignerUI designer)
        {
            Report reportEntity = null;

            if (reportID != null)
                reportEntity = _reportService.GetReportByURI(reportID);

            if (saveAs || reportEntity == null)
            {
                SaveReportDialogCallbackMessage dialogResult = null;
                var msg = new SaveReportMessage(this, (r) =>
                {
                    dialogResult = r;
                    dialogEvent.Set();
                });

                dialogEvent.Reset();

                Messenger.Default.Send(msg);

                dialogEvent.WaitOne();

                if (dialogResult == null || dialogResult.Result != true)
                    return string.Empty;

                reportEntity = _reportService.CreateReport(
                    dialogResult.AssetTypeId, dialogResult.ReportName);
            }

            var report = reportProvider.GetReport();
            _reportService.UpdateReport(reportEntity, report);

            return string.Format("{0}/{1}/{2}",
                AppFramework.Reports.Constants.ReportTypeCustomReport,
                reportEntity.DynEntityConfigId,
                reportEntity.ReportUid);
        }
    }
}
