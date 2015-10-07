using System;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;
using AppFramework.Core.Helpers;
using AppFramework.Core.Properties;

namespace AppFramework.Core.PL
{
    public class AssetAttributeTextbox : TextBox, IAssetAttributeControl
    {
        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {
            Attributes.Add(name, value);
        }

        public AssetAttributeTextbox(AssetAttribute attribute)
        {
            this.AssetAttribute = attribute;
            this.Attributes.Add("style", "width:" + ApplicationSettings.ControlsWidth);
            this.Text = attribute.Value;
            this.Load += new EventHandler(AssetAttributeTextbox_Load);            
            //this.CssClass = string.Format("tmp{0}", attribute.GetConfiguration().Name); // just for using in client JavaScript            
        }

        public void AssetAttributeTextbox_Load(Object sender, EventArgs e)
        {
            this.Enabled = this.AssetAttribute.GetConfiguration().DataType.Editable && this.Editable;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AssetAttribute.GetConfiguration().Parent.AutoGenerateNameType==AppFramework.ConstantsEnumerators.Enumerators.TypeAutoGenerateName.InsertOnly ||
                AssetAttribute.GetConfiguration().Parent.AutoGenerateNameType == AppFramework.ConstantsEnumerators.Enumerators.TypeAutoGenerateName.InsertUpdate)
            {                
                Page.ClientScript.RegisterClientScriptInclude("hint", "/javascript/jquery.hint.js");
                this.Attributes.Add("title", Resources.AutoNameText);
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_hint_" + this.ClientID,
                    "$(function () { $('#" + this.ClientID + "').hint(); });", true);
            }
        }

        #region IAssetAttributeControl Members

        public AssetAttribute GetAttribute()
        {
            AssetAttribute.Value = Formatting.RemoveScriptTags(this.Text.Trim());
            return this.AssetAttribute;
        }

        public bool Editable { get; set; }

        #endregion

    }
}
