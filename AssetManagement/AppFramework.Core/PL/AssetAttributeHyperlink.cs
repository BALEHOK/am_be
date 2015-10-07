using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;

namespace AppFramework.Core.PL
{
    /// <summary>
    /// Displays the hyperlink for attribute type of asset
    /// </summary>
    public class AssetAttributeHyperlink : HyperLink, IAssetAttributeControl
    {
        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {
            Attributes.Add(name, value);
        }

        public AssetAttributeHyperlink(AssetAttribute attribute)
        {
            AssetAttribute = attribute;
        }

        /// <summary>
        /// if it is initialization with complete attribute
        /// </summary>
        /// <param name="e"></param> 
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            #region commented as duplicated
            if (AssetAttribute.ValueAsId.HasValue && AssetAttribute.RelatedAsset != null)
            {
                string identity = string.Empty;
                if (AssetAttribute.ParentAsset.IsHistory)
                {
                    identity = "AssetUID";
                }
                else
                {
                    identity = "AssetID";
                }
                SetControlUI(AssetAttribute.RelatedAsset.GetDisplayName(this.AssetAttribute.GetConfiguration().RelatedAssetTypeAttributeID.Value),
                             string.Format(@"/Asset/View.aspx?{0}={1}&AssetTypeID={2}",
                                            identity,
                                            AssetAttribute.ValueAsId,
                                            AssetAttribute.GetConfiguration().RelatedAssetTypeID));
            }
            else if (string.IsNullOrEmpty(Text))
            {
                SetControlUI(String.Empty, String.Empty);
            }
            #endregion
        }

        private void SetControlUI(string text, string url)
        {
            this.Text = text;
            this.NavigateUrl = url;
        }

        #region IAssetAttributeControl Members

        public AssetAttribute GetAttribute()
        {
            return this.AssetAttribute;
        }

        public bool Editable { get; set; }

        #endregion

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            GridViewRow li = this.NamingContainer as GridViewRow;
            Asset a = li.DataItem as Asset;

            if (a == null)
            {
                throw new NullReferenceException("Cannot bind asset to AssetsGrid row");
            }

            string valStr = string.Format("[{0}].Value", AssetAttribute.GetConfiguration().DBTableFieldName);
            Object value = DataBinder.Eval(li.DataItem, valStr);

            string valAsIdStr = string.Format("[{0}].ValueAsId", AssetAttribute.GetConfiguration().DBTableFieldName);
            Object valueAsId = DataBinder.Eval(li.DataItem, valAsIdStr);

            if (value != null && valueAsId != null)
            {
                long assetId;
                if (long.TryParse(valueAsId.ToString(), out assetId) && assetId > 0)
                {
                    SetControlUI(value.ToString(),
                                 string.Format(@"/Asset/View.aspx?AssetID={0}&AssetTypeID={1}",
                                               valueAsId.ToString(),
                                               AssetAttribute.GetConfiguration().RelatedAssetTypeID));
                }
                else
                {
                    SetControlUI(String.Empty, String.Empty);
                }
            }
        }

    }
}
