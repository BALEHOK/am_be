using AssetSite.admin.Reports.Add;
namespace AssetSite.Reports.Add
{
    using AppFramework.Core.Classes;
    using AppFramework.Core.Classes.Reports;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    public partial class Default : BasePage
    {
        private Report _report;

        protected void Page_Load(object sender, EventArgs e)
        {
            _report = new Report(new AppFramework.Entities.Report(), 
                AuthenticationService, AssetTypeRepository, AssetsService);

            if (!IsPostBack)
            {
                AssetType.DataSource = AssetTypeRepository.GetAllPublished();
                AssetType.DataBind();

                this.BindAttributes(0);
            }

            Save.Attributes.Add("onclick", "window.scrollTo(0,0)");
        }

        /// <summary>
        /// Saves the report
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveReportClick(object sender, EventArgs e)
        {
            long assetTypeId = 0;
            long.TryParse(AssetType.SelectedValue, out assetTypeId);

            var rep = new Report(ReportName.Text, IncludeFinInfo.Checked, assetTypeId, 
                AuthenticationService, AssetTypeRepository, AssetsService);

            foreach (var item in AssetTypes.Items)
            {
                RepeaterItem repeaterItem = item as RepeaterItem;
                var nameControl = repeaterItem.FindControl("AttributeName") as HiddenField;
                string attributeName = nameControl.Value;

                string attributeUidValue = (repeaterItem.FindControl("AttributeUid") as HiddenField).Value;
                long attributeUid = 0;
                long.TryParse(attributeUidValue, out attributeUid);

                var grid = repeaterItem.FindControl("FieldsList") as GridView;

                foreach (GridViewRow row in grid.Rows)
                {
                    long uid = 0;
                    long.TryParse((row.Cells[0].FindControl("UID") as HiddenField).Value, out uid);
                    string name = (row.Cells[1] as DataControlFieldCell).Text;
                    bool isVisible = (row.Cells[2].FindControl("IsVisible") as CheckBox).Checked;
                    bool isFilter = (row.Cells[3].FindControl("IsFilter") as CheckBox).Checked;
                    if (uid != 0)
                    {
                        var attrName = attributeUid == 0 ? name : attributeName + "." + name;   // if attrubute uid is 0 attribute belongs to "main" asset type
                        rep.AddField(attrName, isVisible, isFilter);
                    }
                }
            }

            rep.Save();

            Response.Redirect(string.Format("SelectTemplate.aspx?Uid={0}", rep.UID));
        }

        protected void AssetTypeDataBound(object sender, EventArgs e)
        {
            AssetType.Items.Insert(0, new ListItem((string)GetLocalResourceObject("lblAll"), "0"));
        }

        private void BindAttributes(long assetTypeUid)
        {
            AssetType at = null;
            IEnumerable<AssetTypeViewModel> linkedAssets = null;
            if (assetTypeUid == 0)
            {
                at = AssetTypeRepository.GetGeneralAssetType();
            }
            else
            {
                at = AssetTypeRepository.GetById(assetTypeUid);
            }

            linkedAssets = GetAssetTypes(at);

            AssetTypes.DataSource = linkedAssets;
            AssetTypes.DataBind();
        }

        protected IEnumerable<AssetTypeViewModel> GetAssetTypes(AssetType at)
        {
            IEnumerable<AssetTypeViewModel> linkedAssets = new List<AssetTypeViewModel>() { new AssetTypeViewModel(at.Name, 0, at) };
            var related = at.Attributes
                .Where(atr => atr.DataTypeEnum == AppFramework.ConstantsEnumerators.Enumerators.DataType.Asset
                    && atr.RelatedAssetTypeID.HasValue).OrderBy(a => a.DisplayOrder)
                .Select(a => new AssetTypeViewModel(a.Name, a.UID, AssetTypeRepository.GetById(a.RelatedAssetTypeID.Value)));

            linkedAssets = linkedAssets.Concat(related);

            return linkedAssets;
        }

        protected void AssetTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            long uid = 0;
            long.TryParse(AssetType.SelectedValue, out uid);
            BindAttributes(uid);
        }

        protected void AssetTypes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var grid = e.Item.FindControl("FieldsList") as GridView;
            var assetType = e.Item.DataItem as AssetTypeViewModel;

            if (grid != null && assetType != null)
            {
                grid.DataSource = assetType.AssetType.Attributes.Where(a => a.IsShownOnPanel).OrderBy(a => a.DisplayOrder);
                grid.DataBind();
            }
        }
    }
}
