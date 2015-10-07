using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.Core.Classes;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppFramework.Core.PL
{
    public class AssetHyperLinkTemplate : ITemplate
    {
        public AssetHyperLinkTemplate(Asset asset)
        {
            
        }

        #region ITemplate Members
        public void InstantiateIn(Control container)
        {
            HyperLink link = new HyperLink();
            link.DataBinding += new EventHandler(link_DataBinding);
            container.Controls.Add(link);
        }
        #endregion

        private void link_DataBinding(Object sender, EventArgs e)
        {

        }
    }
}
