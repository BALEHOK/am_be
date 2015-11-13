using AssetManagerAdmin.Model;
using System;
using System.Windows;

namespace AssetManagerAdmin
{
    public class DialogService : IDialogService
    {
        public void ShowError(Exception error, string title)
        {
            ShowMessage(error.Message, title, StatusMessageType.Error);
        }

        public void ShowError(string errorMessage, string title)
        {
            ShowMessage(errorMessage, title, StatusMessageType.Error);
        }

        public void ShowMessage(string message, string title, StatusMessageType status)
        {
            MessageBoxImage icon = MessageBoxImage.None;
            if (status == StatusMessageType.Error)
                icon = MessageBoxImage.Error;
            if (status == StatusMessageType.Info)
                icon = MessageBoxImage.Information;
            if (status == StatusMessageType.Sussess)
                icon = MessageBoxImage.Asterisk;
            MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        }
    }
}
