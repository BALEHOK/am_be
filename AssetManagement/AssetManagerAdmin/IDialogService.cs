using AssetManagerAdmin.Model;
using System;

namespace AssetManagerAdmin
{
    public interface IDialogService
    {
        void ShowError(string errorMessage, string title);
        void ShowError(Exception error, string title);
        void ShowMessage(string message, string title, StatusMessageType status);
    }
}
