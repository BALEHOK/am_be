using AppFramework.Core.Classes;
using AppFramework.Core.PL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;

namespace AssetSite.Controls
{
    public partial class MultipleAssetsEditor : UserControl, IAssetAttributeControl
    {
        [Dependency]
        public IAssetsService AssetsService { get; set; }
        [Dependency]
        public IAssetTypeRepository AssetTypeRepository { get; set; }

        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //mark this so Client-Side that is should handle this 'Input' as multiselect how to handle
            lstSelectedAssets.Attributes.Add("ismultilpleassetslist", "true");
            if (Editable)
            {
                var attrConf = this.AssetAttribute.GetConfiguration();
                var aType = AssetTypeRepository.GetById(attrConf.RelatedAssetTypeID.Value);

                this.DialogContainer.Attributes.Add("title", aType.Name);

                string onselect = "return OnMultipleSelect('" + lstSelectedAssets.ClientID + "','" + lstAssets.ClientID + "','" + DialogContainer.ClientID + "','" + hdfAddedItems.ClientID + "');";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_DlgInitalize_" + this.ClientID, "$(function () {$('#" + this.DialogContainer.ClientID +
                    "').dialog({ autoOpen: false, width: 420, height: 560, buttons: { '" + Resources.Global.CancelText
                    + "':function(){ $(this).dialog('close'); }, 'Ok':function(){ $(this).dialog('close'); " + onselect + " } } });});", true);
                lbtnSearch.Attributes.Add("onclick", "return ShowDialog('" + this.DialogContainer.ClientID + "');");
                lbtnDoSearch.Attributes.Add("onclick", "return OnTextChangedReturnsIdName(" + attrConf.RelatedAssetTypeID + "," + attrConf.RelatedAssetTypeAttributeID + ",'" + tbSearchPattern.ClientID + "','" + lstAssets.ClientID + "');");
                lnkAdd.NavigateUrl = "~/Asset/New/Step2.aspx?atid=" + this.AssetAttribute.Configuration.RelatedAssetTypeID;
                lbtnDelete.Attributes.Add("onclick", "return OnMultipleDelete('" + lstSelectedAssets.ClientID + "','" + hdfDeletedItems.ClientID + "' );");
                lstAssets.Attributes.Add("onscroll", "OnScrollChangedReturnsIdName(this," + attrConf.RelatedAssetTypeID + "," + attrConf.RelatedAssetTypeAttributeID + ",'" + tbSearchPattern.ClientID + "','" + lstAssets.ClientID + "');");
               
                foreach (KeyValuePair<long, string> pair in this.AssetAttribute.MultipleAssets)
                {
                    ListItem item = new ListItem();
                    item.Text = pair.Value;
                    item.Value = pair.Key.ToString();

                    lstSelectedAssets.Items.Add(item);
                    hdfAddedItems.Value += pair.Key.ToString() + ",";
                }
                mvMultipleAssets.SetActiveView(viewEdit);
            }
            else
            {
                this.GetHyperlinksPanel(this.AssetAttribute.MultipleAssets);
                mvMultipleAssets.SetActiveView(viewView);
            }
        }

        private void GetHyperlinksPanel(List<KeyValuePair<long, string>> assets)
        {
            pnlLinks.Controls.Clear();

            foreach (KeyValuePair<long, string> asset in assets)
            {
                pnlLinks.Controls.Add(new HyperLink()
                {
                    Text = asset.Value,
                    NavigateUrl = string.Format(@"/Asset/View.aspx?AssetID={0}&AssetTypeID={1}",
                                                asset.Key, AssetAttribute.GetConfiguration().RelatedAssetTypeID)
                });
                pnlLinks.Controls.Add(new LiteralControl("<br/>"));
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            GridViewRow li = this.NamingContainer as GridViewRow;
            AppFramework.Core.Classes.Asset a = li.DataItem as AppFramework.Core.Classes.Asset;

            if (a == null)
            {
                throw new NullReferenceException("Cannot bind asset to AssetsGrid row");
            }

            Object value = DataBinder.Eval(li.DataItem, string.Format("[{0}].MultipleAssets", AssetAttribute.GetConfiguration().DBTableFieldName));
            if (value != null)
            {
                mvMultipleAssets.SetActiveView(viewView);
                GetHyperlinksPanel(value as List<KeyValuePair<long, string>>);
            }
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

        #region IAssetAttributeControl
        public AssetAttribute GetAttribute()
        {
            //if (!Editable)
            //{
            //    throw new System.Exception("Cannot get the value from non-editable control");
            //}

            AssetAttribute.MultipleAssets.Clear();
            string[] removedIds = hdfDeletedItems.Value.TrimEnd(new char[1] { ',' }).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Distinct().ToArray();
            string[] addedIds = hdfAddedItems.Value.TrimEnd(new char[1] { ',' }).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Distinct().ToArray();

            var currentAT = AssetTypeRepository.GetById(this.AssetAttribute.Configuration.RelatedAssetTypeID.Value);
            var removedAssets = new List<KeyValuePair<long, string>>();
            foreach (string sid in removedIds)
            {
                long id = long.Parse(sid);
                string name = AssetsService.GetAssetById(id, currentAT).Name;
                removedAssets.Add(new KeyValuePair<long, string>(id, name));
            }

            var addedAssets = new List<KeyValuePair<long, string>>();
            foreach (string sid in addedIds)
            {
                long id = long.Parse(sid);
                string name = AssetsService.GetAssetById(id, currentAT).Name;
                addedAssets.Add(new KeyValuePair<long, string>(id, name));
            }

            foreach (KeyValuePair<long, string> pair in addedAssets)
            {
                if (!removedAssets.Contains(pair))
                    this.AssetAttribute.MultipleAssets.Add(pair);
            }

            return AssetAttribute;
        }

        public bool Editable
        {
            get;
            set;
        }
        #endregion
    }
}