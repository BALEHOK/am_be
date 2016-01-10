using GalaSoft.MvvmLight.Messaging;
using System;

namespace AssetManagerAdmin.Infrastructure.Messages
{
    public class OpenReportMessage : NotificationMessageAction<OpenReportDialogCallbackMessage>
    {
        public OpenReportMessage(object sender, Action<OpenReportDialogCallbackMessage> callback)
            : base(sender, "OpenReport", callback)
        {

        }
    }
}
