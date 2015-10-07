using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.PL;
using System;
using System.Web.UI;
using Microsoft.Practices.Unity;

namespace AssetSite.Controls
{
    public partial class AssetDropDownListEx : UserControl, IAssetAttributeControl
    {
        [Dependency]
        public IAuthenticationService AuthenticationService { get; set; }

        [Dependency]
        public IAssetTypeRepository AssetTypeRepository { get; set; }

        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {
            Attributes.Add(name, value);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AssetAttribute != null)
                Configure(AssetAttribute.GetConfiguration());
        }

        public void Configure(AssetTypeAttribute attrConf)
        {
            if (attrConf.RelatedAssetTypeAttributeID == 0 || attrConf.RelatedAssetTypeID == 0)
                return;            

            Page.ClientScript.RegisterClientScriptInclude("highlight", "/javascript/jquery.highlight-3.js");
            Page.ClientScript.RegisterClientScriptInclude("hint", "/javascript/jquery.hint.js");

            this.DialogContainer.Attributes.Add("title", attrConf.Name);
            string onSelect = string.Format("return OnSelect('{0}', '{1}', '{2}', '{3}', '{4}');",
                                            lstAssets.ClientID, hdfSelectedValue.ClientID, hdfSelectedText.ClientID,
                                            tbSelectedAsset.ClientID, DialogContainer.ClientID);
            if (!RegisterStartupScript)
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_DlgInitalize_" + this.ClientID,
                                                            "$(function () { $('#" + this.DialogContainer.ClientID +
                                                            "').dialog({ autoOpen: false, width: 420, height: 560, buttons : { 'Ok' : function(){ $(this).dialog('close'); " + onSelect + " }, " +
                                                            Resources.Global.CancelText + " : function() { $(this).dialog('close'); " +
                                                            " } } }); $('#"
                                                            + tbSearchPattern.ClientID + "').hint(); });", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), this.ClientID + "_DlgInitalize_",
                                                    "$(function(){$(\"div[aria-labelledby='ui-dialog-title-" +
                                                    this.DialogContainer.ClientID + "']\").remove(); $('#" +
                                                    this.DialogContainer.ClientID +
                                                    "').dialog({ autoOpen: false, width: 420, height: 560, buttons : { 'Ok' : function(){ $(this).dialog('close'); "+ onSelect +" }, " +
                                                    Resources.Global.CancelText + " : function() { $(this).dialog('close'); " +
                                                    " } } }); $('#"
                                                    + tbSearchPattern.ClientID + "').hint(); " + "if($(\"div[id='" +
                                                    this.DialogContainer.ClientID + "']\").size()>1){$(\"div[id='" +
                                                    this.DialogContainer.ClientID + "']\").first().remove();}" + "});",
                                                    true);
            }

            lbtnSearch.Attributes.Add("onclick",
                                      "OnOpened(" + attrConf.RelatedAssetTypeID + "," +
                                      attrConf.RelatedAssetTypeAttributeID + ",'" + tbSearchPattern.ClientID + "','" +
                                      lstAssets.ClientID + "'); " +
                                      "return ShowDialog('" + this.DialogContainer.ClientID + "');");

            tbSelectedAsset.Attributes.Add("onkeypress",
                                           // skip tab keypress
                                           "(function() {  getEvent=event.keyCode; if (getEvent != '9') { return ShowDialog('" +
                                           this.DialogContainer.ClientID + "'); } })();");

            lstAssets.Attributes.Add("onkeypress",
                                     "(function() {  getEvent=event.keyCode; if (getEvent == '13') { " + onSelect +
                                     " } })();");
            lstAssets.Attributes.Add("onscroll",
                                     "OnScrollChangedReturnsIdName(this," + attrConf.RelatedAssetTypeID + "," +
                                     attrConf.RelatedAssetTypeAttributeID + ",'" + tbSearchPattern.ClientID + "','" +
                                     lstAssets.ClientID + "');");

            lbtnDoSearch.Attributes.Add("onclick",
                                        "return OnTextChangedReturnsIdName(" + attrConf.RelatedAssetTypeID + "," +
                                        attrConf.RelatedAssetTypeAttributeID + ",'" + tbSearchPattern.ClientID + "','" +
                                        lstAssets.ClientID + "');");

            lnkAdd.Visible = attrConf.RelatedAssetTypeID.HasValue &&
                             AuthenticationService.IsWritingAllowed(AssetTypeRepository.GetById(attrConf.RelatedAssetTypeID.Value));
            lnkAdd.NavigateUrl = "~/Asset/New/Step2.aspx?atid=" + attrConf.RelatedAssetTypeID;
            lbtnDelete.Attributes.Add("onclick",
                                      "return OnSingleDelete('" + tbSelectedAsset.ClientID + "','" +
                                      hdfSelectedValue.ClientID + "');");

            lbtnDelete.Visible = !attrConf.IsRequired;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (this.AssetAttribute != null)
            {
                if (IsPostBack && !string.IsNullOrEmpty(hdfSelectedValue.Value) && !string.IsNullOrEmpty(hdfSelectedText.Value))
                {
                    tbSelectedAsset.Text = hdfSelectedText.Value;
                }
                else
                {
                    if (this.AssetAttribute.RelatedAsset != null)
                    {
                        hdfSelectedValue.Value = this.AssetAttribute.RelatedAsset.ID.ToString();
                        tbSelectedAsset.Text = this.AssetAttribute.RelatedAsset.GetDisplayName(this.AssetAttribute.GetConfiguration().RelatedAssetTypeAttributeID.Value);
                        var rights = AuthenticationService.GetPermission(this.AssetAttribute.RelatedAsset);
                        lnkAdd.Visible = rights.CanWrite();
                    }
                    else
                    {
                        tbSelectedAsset.Text = string.Empty;
                        hdfSelectedValue.Value = "0";
                        lbtnDelete.Visible = false;
                    }
                }
            }
            base.OnPreRender(e);
        }

        protected override object SaveControlState()
        {
            return (object)this.AssetAttribute;
        }

        protected override void LoadControlState(object savedState)
        {
            if (savedState != null)
            {
                this.AssetAttribute = (AssetAttribute)savedState;
            }
        }

        public AssetAttribute GetAttribute()
        {
            if (!String.IsNullOrEmpty(hdfSelectedValue.Value))
            {
                long id = 0;
                long.TryParse(hdfSelectedValue.Value, out id);
                this.AssetAttribute.ValueAsId = id;
            }
            return this.AssetAttribute;
        }

        public bool Editable { get; set; }
        public bool RegisterStartupScript { get; set; }
    }
}