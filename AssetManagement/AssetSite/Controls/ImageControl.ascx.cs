using System;
using System.Web.UI;
using AppFramework.Core.PL;

namespace AssetSite.Controls
{
    public partial class ImageControl : UserControl, IAssetAttributeControl
    {        
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude("fancybox", "/javascript/jquery.fancybox-1.3.4.pack.js");
            if (!string.IsNullOrEmpty(AssetAttribute.Value))
            {
                lnkImage.NavigateUrl = AssetAttribute.Value;
                imageCtrl.ImageUrl = "/ImageGen.ashx?AllowUpSizing=false&MaxWidth=500&MaxHeight=500&width=400&height=400&Constrain=true&image=" + AssetAttribute.Value;

                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_fancybox_" + this.ClientID,
                    "$(function () { $('#" + lnkImage.ClientID + "').fancybox({ 'hideOnContentClick': true }); });", true);
            }
            else
            {
                this.Visible = false;
            }
        }

        public AppFramework.Core.Classes.AssetAttribute GetAttribute()
        {
            return AssetAttribute;
        }

        public bool Editable { get; set; }

        public AppFramework.Core.Classes.AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {            
        }
    }
}