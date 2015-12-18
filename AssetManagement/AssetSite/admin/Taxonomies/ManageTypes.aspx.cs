using AppFramework.Core.Classes;
using System;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;

namespace AssetSite.admin.Taxonomies
{
    public partial class ManageTypes : BasePage
    {
        private long _uid;
        private TaxonomyItem _taxonomyItem;

        [Dependency]
        public ITaxonomyItemService TaxonomyItemService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!long.TryParse(Request.QueryString["Uid"], out _uid) || _uid == 0)
            {
                Response.Redirect("~/admin/Taxonomies/");
            }

            _taxonomyItem = TaxonomyItemService.GetByUid(_uid);

            AssetsTypes.DataSource = TaxonomyItemService.GetAssignedAssetTypes(_taxonomyItem.Base);
            AssetsTypes.DataBind();

            BindFiltergridIfNeeded();

            if (!IsPostBack)
            {
                AllAssetTypes.DataSource = AssetTypeRepository.GetAllPublished();
                AllAssetTypes.DataBind();
            }

            ScriptManager1.RegisterAsyncPostBackControl(FilteredListUpdate);
        }

        protected void Finished_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("~/admin/Taxonomies/TreeEdit.aspx?Uid={0}&Edit=1", _taxonomyItem.TaxonomyUid));
        }

        protected void UpdateFilteredList(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Add all checked assets from filtered data grid to current taxonomy item
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnAddFilteredClick(object sender, EventArgs e)
        {
            foreach (GridViewRow row in AssetFilteredList.Rows)
            {
                HiddenField uidField = row.Cells[0].FindControl("UID") as HiddenField;
                CheckBox Checked = row.Cells[0].FindControl("TypeSelected") as CheckBox;
                if (Checked != null && Checked.Checked)
                {
                    long assetTypeUid;
                    if (long.TryParse(uidField.Value, out assetTypeUid) && assetTypeUid != 0)
                    {
                        TaxonomyItemService.AddAssignedAssetType(_taxonomyItem.Id, assetTypeUid);
                    }
                }
            }

            Response.Redirect(Request.Url.OriginalString);
        }

        /// <summary>
        /// Add selected in dropdown asset type to current taxonomy item
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnAddTypeClick(object sender, EventArgs e)
        {
            long assetTypeUid;
            if (long.TryParse(AllAssetTypes.SelectedValue, out assetTypeUid) && assetTypeUid != 0)
            {
                TaxonomyItemService.AddAssignedAssetType(_taxonomyItem.Id, assetTypeUid);
                Response.Redirect(Request.Url.OriginalString);
            }
        }

        /// <summary>
        /// Removes the selected asset types from assosiation
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void RemoveSelected(object sender, EventArgs e)
        {
            foreach (GridViewRow row in AssetsTypes.Rows)
            {
                var uidField = row.Cells[0].FindControl("UID") as HiddenField;
                var Checked = row.Cells[0].FindControl("AssetToDelete") as CheckBox;
                if (uidField == null || Checked == null || !Checked.Checked)
                {
                    continue;
                }

                long assetTypeUid;
                if (long.TryParse(uidField.Value, out assetTypeUid) && assetTypeUid != 0)
                {
                    TaxonomyItemService.RemoveAssignedAssetType(_taxonomyItem.Id, assetTypeUid);
                }
            }

            Response.Redirect(Request.Url.OriginalString);
        }

        private void BindFiltergridIfNeeded()
        {
            if (string.IsNullOrWhiteSpace(AssetName.Text))
            {
                return;
            }

            AssetFilteredList.DataSource = AssetType.FindByName(AssetName.Text).Take(5);
            AssetFilteredList.DataBind();
        }
    }
}