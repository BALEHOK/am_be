using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppFramework.Core.PL
{
    public class LabelTemplate : ITemplate
    {
        public string Text { get; set; }

        public LabelTemplate() { }

        #region ITemplate Members

        public void InstantiateIn(Control container)
        {
            Label lbl = new Label();
            lbl.Text = Text;
            container.Controls.Add(lbl);
        }

        #endregion
    }
}