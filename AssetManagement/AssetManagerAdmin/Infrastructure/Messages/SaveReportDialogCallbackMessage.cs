namespace AssetManagerAdmin.Infrastructure.Messages
{
    public class SaveReportDialogCallbackMessage
    {
        public bool? Result { get; set; }

        public string ReportName { get; set; }

        public long AssetTypeId { get; set; }
    }
}
