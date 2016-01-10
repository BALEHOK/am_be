using GalaSoft.MvvmLight.Messaging;
using System;

namespace AssetManagerAdmin.Infrastructure.Messages
{
    public class SaveReportMessage : NotificationMessageAction<SaveReportDialogCallbackMessage>
    {
        public SaveReportMessage(object sender, Action<SaveReportDialogCallbackMessage> callback)
            : base(sender, "SaveReport", callback)
        {

        }
    }
}
