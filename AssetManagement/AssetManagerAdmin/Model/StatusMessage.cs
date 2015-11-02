using System;

namespace AssetManagerAdmin.Model
{
    public enum StatusMessageType
    {
        Info,
        Sussess,
        Error
    }

    public class StatusMessage
    {
        public string Message { get; set; }

        public string Title { get; set; }

        public StatusMessageType Status { get; set; }

        public StatusMessage()
        {

        }

        public StatusMessage(Exception exception)
        {
            Message = exception.InnerException != null
                ? exception.InnerException.Message
                : exception.Message;
            Status = StatusMessageType.Error;
            Title = "Error occured";
        }
    }

}
