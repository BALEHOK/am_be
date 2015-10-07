using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using AppFramework.Core.Classes;

namespace AppFramework.Core.PL.Search
{
    [Serializable()]
    public class RefinementLink : System.Web.UI.UserControl
    {
        public event CommandEventHandler Command;

        public enum RefinementType
        {
            ByAssetType,
            ByCategory
        }

        public RefinementLink(long id, string title, RefinementType re)
        {
            this.ID = re.ToString() + id;
            this.Controls.Add(new Label() { Text = title });
            this.Controls.Add(new LiteralControl("&nbsp;("));
            LinkButton undo = new LinkButton()
            {
                Text = (string)GetGlobalResourceObject("Global", "lnkUndo"),
                CommandName = re.ToString(),
                CommandArgument = id.ToString(),
            };
            undo.Command += new CommandEventHandler(undo_Command);
            this.Controls.Add(undo);
            this.Controls.Add(new LiteralControl(")<br />"));
        }

        protected void undo_Command(object sender, CommandEventArgs e)
        {
            if (Command != null)
                Command(this, e);
        }
    }
}
