using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using AppFramework.Core.Classes;

namespace AppFramework.Core.PL
{
    /// <summary>
    /// GridView Template for asset
    /// </summary>
    public class AssetItemTemplate : ITemplate
    {

        private Asset _asset;
        private AssetAttribute _attribute;

        /// <summary>
        /// Template constructor
        /// </summary>
        /// <param name="asset">Asset to render</param>
        /// <param name="attribute">Assetattribute to show</param>
        public AssetItemTemplate(Asset asset, AssetAttribute attribute)
        {
            this._asset = asset;
            this._attribute = attribute;
        }
        
        #region ITemplate Members
        public void InstantiateIn(Control container)
        {
            // asset UID
            Literal l = new Literal();
            l.DataBinding += new EventHandler(Literal_DataBinding);
            
            // asset URL
            HyperLink hl = new HyperLink();
            hl.DataBinding +=new EventHandler(Hyperlink_DataBinding);

            //container.Controls.Add(l);
            container.Controls.Add(hl);
        }
        #endregion

        private void Literal_DataBinding(Object sender, EventArgs e)
        {
            Literal l = sender as Literal;
            GridViewRow li = l.NamingContainer as GridViewRow;

            string AttrValue = string.Format("[{0}].Value", "DynEntityUid");
                                
            if (!object.Equals(li, null))
            {
                object tmp = DataBinder.Eval(li.DataItem, AttrValue);
                if (!object.Equals(tmp, null))
                    l.Text = tmp.ToString();
            }

        }

        private void Hyperlink_DataBinding(Object sender, EventArgs e)
        {
            HyperLink hl = sender as HyperLink;
            GridViewRow li = hl.NamingContainer as GridViewRow;

            string assetTypeUid = this._attribute.GetConfiguration().AssetTypeUID.ToString();
            string assetUID = string.Format("[{0}].Value", "DynEntityUid");

            string LinkText = string.Format("[{0}].Value", this._attribute.GetConfiguration().DBTableFieldName);
            if (!object.Equals(li, null))
            {
                object tmp = DataBinder.Eval(li.DataItem, LinkText);
                if (!object.Equals(tmp, null))
                {
                    hl.Text = tmp.ToString();
                    hl.NavigateUrl = String.Format("~/Asset/View.aspx?AssetUID={0}&AssetTypeUID={1}",
                                                    DataBinder.Eval(li.DataItem, assetUID),
                                                    assetTypeUid
                                                    );
                }
            }

        }

        
    }
}
