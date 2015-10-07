using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace AppFramework.Core.PL
{
    /// <summary>
    /// Defines command button for AssetsGrid
    /// </summary>
    public class AssetsGridCommandButton : System.Web.UI.WebControls.HyperLink
    {
        public enum ButtonType
        {
            View,
            Edit,
            Delete
        }

        public ButtonType Type { get; set; }

        public AssetsGridCommandButton() { }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            switch (Type)
            {
                case ButtonType.View:
                    this.ImageUrl = "/images/buttons/zoom.png";
                    break;

                case ButtonType.Edit:
                    this.ImageUrl = "/images/buttons/edit.png";                    
                    break;

                case ButtonType.Delete:
                    this.ImageUrl = "/images/buttons/delete.png";
                    break;

                default:
                    this.ImageUrl = "/images/buttons/zoom.png";
                    break;
            }
            this.CssClass = "gridbutton";
        }
    }
}
