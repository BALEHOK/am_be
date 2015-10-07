using AppFramework.Core.Classes;
using System;
using System.Web.UI;
using Microsoft.Practices.Unity;

namespace AssetSite.Controls
{
    public partial class AssetsList : UserControl
    {
        [Dependency]
        public IAssetTypeRepository AssetTypeRepository { get; set; }

        public string AssetTypeName { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            AssetType usersType = AssetTypeRepository.FindByName(this.AssetTypeName);
            if (usersType == null) return;

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_DlgInitalize_" + this.ClientID, "$(function () {$('#" + this.DialogContainer.ClientID + "').dialog({ autoOpen: false, width: 420, height: 520 });});", true);
            lbtnSearch.Attributes.Add("onclick", "return ShowDialog('" + this.DialogContainer.ClientID + "');");
            lbtnOK.Attributes.Add("onclick", "return OnSelect('" + lstAssets.ClientID + "','" + hdfSelectedValue.ClientID + "','" + hdfSelectedText.ClientID + "','" + tbSelectedAsset.ClientID + "','" + DialogContainer.ClientID + "');");
            tbSearchPattern.Attributes.Add("onchange", "return OnTextChanged(" + usersType.UID + "," + usersType["Name"].UID + ",'" + tbSearchPattern.ClientID + "','" + lstAssets.ClientID + "');");
        }

        protected override object SaveControlState()
        {
            return (object)this.AssetTypeName;
        }
        protected override void LoadControlState(object savedState)
        {
            if (savedState != null)
                this.AssetTypeName = savedState.ToString();
            else
                this.AssetTypeName = string.Empty;
        }
    }
}