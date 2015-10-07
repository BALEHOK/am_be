using System;
using System.Web.UI;
using AppFramework.Core.Classes;
using AppFramework.Core.Helpers;
using AppFramework.Core.PL;

namespace AssetSite.Controls
{
    public partial class RichtextControl : UserControl, IAssetAttributeControl
    {
        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {
            CKEditor1.Attributes.Add(name, value);
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CKEditor1.Text = AssetAttribute.Value;
                //txtRichTextBox.Text = AssetAttribute.Value;
                //string locale = CookieWrapper.Language.Split(new char[] { '-' })[0].ToLower();
                //Page.ClientScript.RegisterClientScriptInclude("ckeditor", "/client/ckeditor/ckeditor.js");
                //Page.ClientScript.RegisterClientScriptInclude("ckeditor_jquery", "/client/ckeditor/adapters/jquery.js");
                //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_RichText_" + this.ClientID,
                //          "$(function () { " +
                //               string.Format("InitRichText('{0}', '{1}');", this.txtRichTextBox.ClientID, locale) +
                //           "});", true);
            }
        }

        public AppFramework.Core.Classes.AssetAttribute GetAttribute()
        {
            AssetAttribute.Value = Formatting.RemoveScriptTags(CKEditor1.Text);
            return AssetAttribute;
        }

        public bool Editable { get; set; }
    }
}