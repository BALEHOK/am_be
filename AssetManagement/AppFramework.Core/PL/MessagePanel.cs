using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using AppFramework.Core.Classes.IE;

namespace AppFramework.Core.PL
{
    public enum MessageStatus
    {
        Error,
        Warning,
        Normal
    }

    public struct MessageDefinition
    {
        public MessageStatus Status;
        public string Message;        
    }

    public static class Extensions
    {
        public static void Add(this  List<MessageDefinition> Messages, StatusInfo status)
        {
            foreach (string err in status.Errors)
            {
                Messages.Add(new MessageDefinition() { Message = err, Status = MessageStatus.Error });
            }
            foreach (string wrn in status.Warnings)
            {
                Messages.Add(new MessageDefinition() { Message = wrn, Status = MessageStatus.Warning });
            }
        }        
    }

    public class MessagePanel : Panel
    {

        public List<MessageDefinition> Messages { get; set; }

        public MessagePanel() 
        {
            Messages = new List<MessageDefinition>();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnInit(e);

            foreach (MessageDefinition msg in Messages)
            {
                SetMessage(msg.Message, msg.Status);                   
            }          
        }

        public void SetMessage(string message, MessageStatus status)
        {
            string skinId = status.ToString().ToLower();            

            this.Controls.Add(new Label()
                {
                    Text = message,
                    SkinID = skinId
                });

            this.Controls.Add(new LiteralControl("<br/>"));
        }
    }
}
