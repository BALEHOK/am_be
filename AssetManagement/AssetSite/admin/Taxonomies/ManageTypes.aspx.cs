using AppFramework.Core.Classes;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace AssetSite.admin.Taxonomies
{
    public partial class ManageTypes : BasePage
    {
        private long _uid;
        private TaxonomyItem _taxonomyItem;
        private readonly ITaxonomyItemService _taxonomyItemService;

        public ManageTypes()
        {
            _taxonomyItemService = new TaxonomyItemService(AuthenticationService, AssetTypeRepository, UnitOfWork);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!long.TryParse(Request.QueryString["Uid"], out _uid) || _uid == 0)
            {
                Response.Redirect("~/admin/Taxonomies/");
            }

            _taxonomyItem = _taxonomyItemService.GetByUid(_uid);

            if (!IsPostBack)
            {
                AssetsTypes.DataSource = _taxonomyItemService.GetAssignedAssetTypes(_taxonomyItem.Base);
                AssetsTypes.DataBind();

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
            AssetFilteredList.DataSource = AssetType.FindByName(AssetName.Text).Take(5);
            AssetFilteredList.DataBind();
        }

        /// <summary>
        /// Add all checked assets from filtered data grid to current taxonomy item
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnAddFilteredClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
            foreach (GridViewRow row in AssetFilteredList.Rows)
            {
                HiddenField uidField = row.Cells[0].FindControl("UID") as HiddenField;
                CheckBox Checked = row.Cells[0].FindControl("TypeSelected") as CheckBox;
                if (Checked != null && Checked.Checked)
                {
                    long uid = 0;
                    if (long.TryParse(uidField.Value, out uid) && uid != 0)
                    {
                        //_taxonomyItem.AssignedAssetTypes.Add(AssetType.GetByUID(uid));
                    }
                }
            }

            //_taxonomyItem.Save();
            Response.Redirect(Request.Url.OriginalString);
        }

        /// <summary>
        /// Add selected in dropdown asset type to current taxonomy item
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnAddTypeClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
            long id = 0;
            //if (long.TryParse(AllAssetTypes.SelectedValue, out id) && id != 0)
            //{
            //    _taxonomyItem.AssignedAssetTypes.Add(AssetType.GetByID(id));
            //    _taxonomyItem.Save();
            //    Response.Redirect(Request.Url.OriginalString);
            //}
        }

        /// <summary>
        /// Removes the selected asset types from assosiation
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void RemoveSelected(object sender, EventArgs e)
        {
            throw new NotImplementedException();
            //bool hadChanged = false;
            //foreach (GridViewRow row in AssetsTypes.Rows)
            //{
            //    var uidField = row.Cells[0].FindControl("UID") as HiddenField;
            //    var Checked = row.Cells[0].FindControl("AssetToDelete") as CheckBox;
            //    if (uidField == null || Checked == null || !Checked.Checked) continue;
            //    long uid = 0;
            //    if (long.TryParse(uidField.Value, out uid) && uid != 0)
            //    {
            //        var type = _taxonomyItem.AssignedAssetTypes.FirstOrDefault(at => at.UID == uid);
            //        if (type != null)
            //        {
            //            _taxonomyItem.AssignedAssetTypes.Remove(type);
            //            hadChanged = true;
            //        }
            //    }
            //}

            //if (hadChanged)
            //    _taxonomyItem.Save();

            Response.Redirect(Request.Url.OriginalString);
        }
    }
}