using AssetManagerAdmin.Infrastructure;
using AssetManagerAdmin.Infrastructure.Messages;
using DevExpress.Xpf.Reports.UserDesigner;
using GalaSoft.MvvmLight.Messaging;

namespace AssetManagerAdmin.ViewModels
{
    public class ReportBuilderViewModel : ToolkitViewModelBase
    {
        public IReportStorage ReportStorage { get; private set; }

        public ReportBuilderViewModel(
            IAppContext context,
            IReportStorage reportStorage)
            : base(context)
        {
            ReportStorage = reportStorage;

            Messenger.Default.Register<OpenReportMessage>(this, (m) =>
            {
                var dialog = new View.OpenReportDialog();
                var result = dialog.ShowDialog();
                var dialogContext = (dialog.DataContext as OpenReportDialogViewModel);
                m.Execute(new OpenReportDialogCallbackMessage
                {
                    Report = dialogContext != null
                        ? dialogContext.SelectedReport
                        : null,
                    Result = result
                });
            });

            Messenger.Default.Register<SaveReportMessage>(this, (m) =>
            {
                var dialog = new View.SaveReportDialog();
                var result = dialog.ShowDialog();
                var dialogContext = (dialog.DataContext as SaveReportDialogViewModel);
                m.Execute(new SaveReportDialogCallbackMessage
                {
                    AssetTypeId = result == true && dialogContext != null
                        ? dialogContext.ReportAssetType.Id.Value
                        : 0,
                    ReportName = result == true && dialogContext != null
                        ? dialogContext.ReportName
                        : string.Empty,
                    Result = result
                });
            });
        }

        public override void Cleanup()
        {
            base.Cleanup();
            Messenger.Default.Unregister<OpenReportMessage>(this);
            Messenger.Default.Unregister<SaveReportMessage>(this);
        }
    }
}
