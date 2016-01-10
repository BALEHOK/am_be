using AppFramework.Entities;

namespace AssetManagerAdmin.Infrastructure.Messages
{
    public class OpenReportDialogCallbackMessage
    {
        public bool? Result { get; set; }

        public Report Report { get; set; }
    }
}
