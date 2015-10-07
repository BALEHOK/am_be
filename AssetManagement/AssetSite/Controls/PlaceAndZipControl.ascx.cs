using System;
using System.Web.UI;
using AppFramework.Core.Classes;
using AppFramework.Core.PL;

namespace AssetSite.Controls
{
    public partial class PlaceAndZipControl : UserControl, IAssetAttributeControl
    {
        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {            
        }

        public string SelectedText {
            get
            {
                return hdfSelectedText.Value;
            }
            set
            {
                hdfSelectedText.Value = value;
                tbSelection.Text = value;
            }
        }
        
        protected void initControl()
        {
            hdfSelectedText.Value = "";

            if (this.AssetAttribute != null)
            {
                string onselect = "return OnPOZSelect('" + lstResults.ClientID + "','" +
                    hdfSelectedValue.ClientID + "','" + hdfSelectedText.ClientID + "','" + tbSelection.ClientID + "','" + DialogContainer.ClientID + "');";

                ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "highlight", "/javascript/jquery.highlight-3.js");
                ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "zips_and_places", "/javascript/pnz.js");

                ScriptManager.RegisterStartupScript(this, this.GetType(), "_DlgInitalize_" + this.ClientID, "$(function () {$('#" +
                    this.DialogContainer.ClientID + "').dialog({ autoOpen: false, width: 420, height: 540, " +
                    " buttons: { '" + Resources.Global.CancelText + "':function(){ $(this).dialog('close'); }, 'Ok':function(){ $(this).dialog('close'); " + onselect + " } } });}); if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();", true);

                lbtnSearch.Attributes.Add("onclick", "return ShowDialog('" + this.DialogContainer.ClientID + "');");
                this.DialogContainer.Attributes.Add("title", this.AssetAttribute.GetConfiguration().Name);
                tbSelection.Attributes.Add("onkeypress", "return ShowDialog('" + this.DialogContainer.ClientID + "');");
                lbtnDoSearch.Attributes.Add("onclick", "return OnSearchPOZ('" + tbSearchPattern.ClientID + "','" + lstResults.ClientID + "');");
                tbSelection.Text = AssetAttribute.Value;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "dynmaic_loader", "if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();", true);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            initControl();
        }

        #region IAssetAttributeControl
        public AssetAttribute GetAttribute()
        {
            if (!String.IsNullOrEmpty(hdfSelectedValue.Value))
            {
                long id = 0;
                long.TryParse(hdfSelectedValue.Value, out id);
                this.AssetAttribute.ValueAsId = id;
                this.AssetAttribute.Value = hdfSelectedText.Value;
            }

            return this.AssetAttribute;
        }

        public bool Editable
        {
            get;
            set;
        }
        #endregion

        #region Save state
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override object SaveControlState()
        {
            object baseSate = base.SaveControlState();
            return new Pair(baseSate, (object)this.AssetAttribute);
        }

        protected override void LoadControlState(object savedState)
        {
            Pair value = savedState as Pair;
            if (value != null)
            {
                this.AssetAttribute = value.Second as AssetAttribute;
            }
        }
        #endregion
    }
}