using System.Web.Script.Serialization;
using System.Web.Services;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using Microsoft.Practices.Unity;

namespace AssetSite.Asset
{
    public partial class Edit : AssetController
    {
        [Dependency]
        public IDynamicListsService DynamicListsService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {            
            long assetTypeUID = 0;
            long assetUID = 0;

            if (Request.QueryString["AssetTypeUID"] != null
                && Request.QueryString["AssetUID"] != null)
            {
                long.TryParse(Request.QueryString["AssetTypeUID"].ToString(), out assetTypeUID);
                long.TryParse(Request.QueryString["AssetUID"].ToString(), out assetUID);
                long? currentTypeUid = AssetFactory.GetCurrentAssetTypeUid(assetTypeUID);
                if (currentTypeUid != null)
                    assetTypeUID = currentTypeUid.Value;
            }
            else
            {
                Response.Redirect("~/AssetView.aspx");
            }
            
            AssetType = AssetTypeRepository.GetByUid(assetTypeUID);
            Asset = AssetsService.GetAssetByUid(assetUID, AssetType);

            // reading and writing
            if (Asset == null || !AuthenticationService.GetPermission(Asset).CompareWithMask(Permission.RWDD.GetCode()))
            {
                Response.Redirect("~/AssetView.aspx");
            }

            HtmlLink link = new HtmlLink();
            link.Attributes.Add("type", "text/css");
            link.Attributes.Add("rel", "stylesheet");
            link.Attributes.Add("href", this.LayoutCssClass());
            this.Header.Controls.Add(link);

            CheckRestoreState(RestoreAssetMessage);
            
            AssetAttributePanel.Asset = Asset;            

            var buttonCollection = new List<Enumerators.ToolbarButtonType>();
            buttonCollection.AddRange(new Enumerators.ToolbarButtonType[]
                {
                    Enumerators.ToolbarButtonType.Save,
                    Enumerators.ToolbarButtonType.SaveAndAdd, 
                    Enumerators.ToolbarButtonType.Undo,
                });

            buttonCollection.ForEach(b =>
                {
                    toolbar.ButtonCollection.Add(b);
                    bottomtoolbar.ButtonCollection.Add(b);
                });

            toolbar.Options.Add("AssetUID", Asset.UID.ToString());
            toolbar.Options.Add("AssetID", Asset.ID.ToString());
            toolbar.Options.Add("AssetTypeUID", Asset.GetConfiguration().UID.ToString());
            toolbar.Options.Add("AssetTypeID", Asset.GetConfiguration().ID.ToString());
            toolbar.Visible = true;

            bottomtoolbar.Options.Add("AssetUID", Asset.UID.ToString());
            bottomtoolbar.Options.Add("AssetID", Asset.ID.ToString());
            bottomtoolbar.Options.Add("AssetTypeUID", Asset.GetConfiguration().UID.ToString());
            bottomtoolbar.Options.Add("AssetTypeID", Asset.GetConfiguration().ID.ToString());
            bottomtoolbar.Visible = true;

            screensPanel.Visible = ApplicationSettings.ApplicationType != ApplicationType.AssetManager;
            screensPanel.AssetType = AssetType;
            screensPanel.Asset = Asset;

            if (!IsPostBack)
            {
                litRevision.Text = string.Format("[r.{0} - {1}]", Asset[AttributeNames.Revision].Value,
                                                 Asset[AttributeNames.UpdateDate].Value);

                var taxonomyItems = UnitOfWork.TaxonomyItemRepository.GetTaxonomyItemsByAssetTypeId(AssetType.ID);
                foreach (var item in taxonomyItems
                  .Where(ti => ti.Taxonomy.IsActive && ti.Taxonomy.ActiveVersion))
                {
                    bool isCategory = false;
                    string path = this.GetParentName(item);

                    if (path.Contains("C>"))
                    {
                        isCategory = true;
                        path = path.Replace("C>", string.Empty);
                    }

                    IEnumerable<string> tempPath =
                        path.Split(new char[1] {'>'}, StringSplitOptions.RemoveEmptyEntries).Reverse();
                    string nPath = string.Empty;
                    foreach (string pathElem in tempPath)
                    {
                        nPath += pathElem + " > ";
                    }

                    nPath += "<a href='../AssetView.aspx?AstType=" + AssetType.ID + "'>" + AssetType.Name + "</a>";

                    if (isCategory)
                        litCategoryPath.Text += nPath + "<br/>";
                    else
                        litTaxonomies.Text += nPath + "<br/>";
                }
            }
        }

        private string GetParentName(AppFramework.Entities.TaxonomyItem item)
        {
            if (item.ParentItem == null)
            {
                if (item.Taxonomy.IsCategory)
                    return "C>" + item.Name;
                return item.Name;
            }
            if (item.Taxonomy.IsCategory)
                return "C>" + item.Name + ">" + this.GetParentName(item.ParentItem);
            return item.Name + ">" + this.GetParentName(item.ParentItem);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Save(AssetAttributePanel);
        }

        protected void btnSaveAndAdd_Click(object sender, EventArgs e)
        {
            Save(AssetAttributePanel, "~/Asset/New/Step2.aspx?atid=" + AssetType.ID, false);
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AssetView.aspx");
        }
    }
}
