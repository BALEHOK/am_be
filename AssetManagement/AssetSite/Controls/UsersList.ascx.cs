using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;
using System;
using System.Web.UI;
using Microsoft.Practices.Unity;

namespace AssetSite.Controls
{
    public partial class UsersList : UserControl
    {
        [Dependency]
        public IAssetTypeRepository AssetTypeRepository { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var usersType = AssetTypeRepository.FindByName("User");
            if (usersType == null) return;

            Page.ClientScript.RegisterClientScriptInclude("highlight", "/javascript/jquery.highlight-3.js");
            Page.ClientScript.RegisterClientScriptInclude("hint", "/javascript/jquery.hint.js");

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_DlgInitalize_" + this.ClientID, 
                "$(function () {$('#" + this.DialogContainer.ClientID +
                "').dialog({ autoOpen: false, width: 420, height: 620, buttons : { 'Ok' : function(){ return OnSelect('" + lstAssets.ClientID + "','" + hdfSelectedValue.ClientID + "','" + hdfSelectedText.ClientID + "','" + tbSelectedAsset.ClientID + "','" + DialogContainer.ClientID + "'); }, '" +
                Resources.Global.CancelText + "' : function(){ $(this).dialog('close'); },  } }); " +
                "$('#" + tbSearchPattern.ClientID + "').hint(); });", true);
                                                           
            lbtnSearch.Attributes.Add("onclick", "return ShowDialog('" + this.DialogContainer.ClientID + "');");
            lbtnDoSearch.Attributes.Add("onclick", "return OnTextChangedReturnsIdName(" + usersType.ID + "," + usersType[AttributeNames.Name].ID + ",'" + tbSearchPattern.ClientID + "','" + lstAssets.ClientID + "');");
        }

        public string SelectedTextInputBoxClientId
        {
            get { return tbSelectedAsset.ClientID; }
        }

        public string SelectedValueInputBoxClientId
        {
            get { return hdfSelectedValue.ClientID; }
        }

        public long? GetSelectedUserId()
        {
            long userId;
            if (!long.TryParse(hdfSelectedValue.Value, out userId))
                return null;
            return userId;
        }
    }
}