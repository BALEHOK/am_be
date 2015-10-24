using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;
using AppFramework.Entities;

namespace AssetSite.Controls
{
    public partial class AssetTypeChildren : System.Web.UI.UserControl
    {
        public long AssetTypeId
        {
            get
            {
                return ViewState["AssetType_ID"] == null ? -1 : (long)ViewState["AssetType_ID"];
            }
            set
            {
                ViewState["AssetType_ID"] = value;
            }
        }

        public long AssetId
        {
            get
            {
                return ViewState["Asset_Uid"] == null ? -1 : (long)ViewState["Asset_Uid"];
            }
            set
            {
                ViewState["Asset_Uid"] = value;
            }
        }
        private List<f_cust_GetChildAssets_Result> _array;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && this.Visible && AssetTypeId != -1)
            {
                _array = AssetFactory.GetChildAssets(AssetTypeId);
                rAssetTypes.DataSource = _array;
                rAssetTypes.DataBind();
            }
        }

        protected void rAssetTypes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = e.Item.DataItem as f_cust_GetChildAssets_Result;
                HyperLink lnkAssetType = e.Item.FindControl("linkAssetType") as HyperLink;
                string name = new TranslatableString(item.AssetTypeName).GetTranslation();
                if (_array.Any(a => a.DynEntityConfigId == item.DynEntityConfigId))
                {
                    name += "(" + new TranslatableString(item.AttributeName).GetTranslation() + ")";
                }
                lnkAssetType.Text = name;               
                lnkAssetType.NavigateUrl = 
                    string.Format(
                        "~/Search/ResultByType.aspx?AttributeUId={0}&TypeUID={1}&AssetId={2}&AssetTypeId={3}", 
                        item.DynEntityAttribConfigUid.ToString(),
                        item.DynEntityConfigUid.ToString(), 
                        AssetId, 
                        AssetTypeId);
            }
        }      
    }
}