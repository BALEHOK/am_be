using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace AppFramework.Core.PL
{
    [DefaultProperty("Message")]
    [ToolboxData("<{0}:HelpButton runat=server></{0}:HelpButton>")]
    public class HelpButton : System.Web.UI.WebControls.Image
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public Levels Level { get; set; }

        public enum Levels
        {
            Error,
            Warning,
            Info
        }

        private string _filename
        {
            get
            {
                string result = string.Empty;
                switch (Level)
                {
                    case Levels.Error:
                        result = "error";
                        break;

                    case Levels.Info:
                        result = "question";
                        break;

                    case Levels.Warning:
                        result = "attention";
                        break;

                    default:
                        result = "question";
                        break;
                }
                return result;
            }
        }


        public HelpButton()
        {
            ImageUrl = "~/images/buttons/question.png";
            ToolTip="Show help";
            AlternateText = "help";            
            CssClass = "helpbutton";            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Attributes.Add("onclick",
                "showHint('" + Title + "','" 
                                    + Message + "','" 
                                    + Level.ToString().ToLower() + "', '"
                                    + ClientID + "', event.pageY)");             
            
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            string scriptKey = "messageboxHandler";
            if (!Page.ClientScript.IsStartupScriptRegistered(typeof(Page), scriptKey) && this.Enabled)                         
            {
                string scriptBlock =
                   @"<script language=""javascript"">
                   <!--
                       $(document).ready(function() {
                            $('body').delegate('mouseover', '.helpbutton', function() {
                                $(this).attr('src', '/images/buttons/question_hover.png');
                            });
                            $('body').delegate('mouseout', '.helpbutton', function() {
                                $(this).attr('src', '/images/buttons/question.png');
                            });                                                     
                       });
  
                       function showHint(title, text, level, invoked, eventY) {
                            
                            var speed = 200;            
                            var messagebox = $('#messagebox');
                            var hidden = $(' #invokedBy', messagebox);

                            if (messagebox == null) return;

                            var margin = (eventY - messagebox.height()) > 0 
                                ? eventY - messagebox.height() - 20 : messagebox.css('margin-top');
                            messagebox.css('margin-top', margin); 

                            if ((hidden.val() == invoked) 
                                && (messagebox.css('display') != 'none')) {
                                messagebox.hide(speed);
                                return;
                            }
                            
                            if (messagebox.css('display') == 'none') {                             
                                messagebox.show(speed);                                     
                            }

                            var icon = $(' #icon', messagebox);                       
                            $(' #title', messagebox).text(title);
                            $(' #statusText', messagebox).text(text);             
                            switch (level) {
                                case 'error':
                                    icon.css(""background-image"", ""url('/images/buttons/attention.png')"");
                                    break;
                                case 'warning':
                                    icon.css(""background-image"", ""url('/images/buttons/question.png')"");
                                    break;
                                case 'info':
                                    icon.css(""background-image"", ""url('/images/buttons/question.png')"");
                                    break;
                                case 'default':
                                    break;
                            }
                            hidden.val(invoked);                                                   
                        }
                   // -->
                   </script>";
                Page.ClientScript.RegisterStartupScript(typeof(Page), scriptKey, scriptBlock);
            }  
        }
    }
}
