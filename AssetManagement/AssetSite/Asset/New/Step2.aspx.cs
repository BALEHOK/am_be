using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Barcode;
using AppFramework.Core.Classes.IE.Adapters;
using AssetSite.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using Microsoft.Practices.Unity;

namespace AssetSite.Asset.New
{
    public partial class Step2 : AssetController
    {
        [Dependency]
        public IBarcodeProvider BarcodeProvider { get; set; }
        [Dependency]
        public IAssetTemplateService AssetTemplateService { get; set; }
        [Dependency]
        public IDynamicListsService DynamicListsService { get; set; }

        private long _assetTypeId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            long.TryParse(Request.QueryString["atid"], out _assetTypeId);

            if (_assetTypeId > 0)
            {
                AssetType = AssetTypeRepository.GetById(_assetTypeId);
                Asset = AssetsService.CreateAsset(AssetType);

                // deny to create some assets for non-admin users                
                if (!AuthenticationService.IsWritingAllowed(AssetType))
                {
                    Response.Redirect("~/Asset/New/Step1.aspx");
                }

                panelTitle.Text = AssetType.Name;

            }
            else
            {
                Response.Redirect("~/Asset/New/Step1.aspx");
            }

            var buttonCollection = new List<Enumerators.ToolbarButtonType>();
            buttonCollection.AddRange(new Enumerators.ToolbarButtonType[]
                {
                    Enumerators.ToolbarButtonType.Save, Enumerators.ToolbarButtonType.SaveAndAdd, Enumerators.ToolbarButtonType.Template,
                    Enumerators.ToolbarButtonType.Undo,
                });
            buttonCollection.ForEach(b =>
            {
                toolbar.ButtonCollection.Add(b);
                bottomtoolbar.ButtonCollection.Add(b);
            });
            bottomtoolbar.Visible = toolbar.Visible = true;

            var link = new HtmlLink();
            link.Attributes.Add("type", "text/css");
            link.Attributes.Add("rel", "stylesheet");
            link.Attributes.Add("href", this.LayoutCssClass());
            this.Header.Controls.Add(link);

            

            CheckRestoreState(RestoreAssetMessage);

            AssetTemplates.Asset = Asset;
            AssetTemplates.AssetType = AssetType;
            if (!IsPostBack)
            {
                var templates = AssetTemplateService.GetTemplatesByAssetTypeUid(this.AssetType.UID);
                if (templates.Any())
                {
                    AssetTemplates.BindData(templates);
                    AssetTemplates.Visible = true;
                }
            }

            AssetAttributePanel.Asset = Asset;

            if (!string.IsNullOrEmpty(Request.QueryString["tid"]) && !IsPostBack)
            {
                long templateID;
                if (long.TryParse(Request.QueryString["tid"], out templateID))
                {
                    var template = AssetTemplateService.GetTemplateByID(this.AssetType.UID, templateID);
                    if (template != null)
                        AssetAttributePanel.Asset = template.Template;
                }
            }

            #region SOBenBUB

            screensPanel.Visible = ApplicationSettings.ApplicationType != ApplicationType.AssetManager;
            screensPanel.AssetType = AssetType;
            screensPanel.Asset = Asset;

            #endregion
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            Save(AssetAttributePanel);
        }

        protected void btnSaveAndAdd_Click(object sender, EventArgs e)
        {
            Save(AssetAttributePanel, Request.Url.OriginalString);
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AssetView.aspx");
        }

        protected void Restore_Click(object sender, EventArgs e)
        {
            var adapter = new XMLToAssetsAdapter(
                AssetsService,
                AssetTypeRepository,
                LinkedEntityFinder,
                BarcodeProvider);
            var impAsset = adapter.GetEntities(this.RestoreFileName, this.AssetType).SingleOrDefault();
            if (impAsset != null)
                AssetAttributePanel.Asset = impAsset;
        }

        protected void btnSaveTemplate_Click(object sender, EventArgs e)
        {
            AppFramework.Core.Classes.Asset asset;
            IDictionary<AssetAttribute, AppFramework.Core.Classes.Asset> dependencies;
            var isValid = AssetAttributePanel.TryGetValidAssetWithDependencies(out asset, out dependencies);

            // checks if this asses can be saved or not
            if (AuthenticationService.GetPermission(AssetAttributePanel.Asset).CanWrite())
            {
                if (isValid)
                {
                    var template = new AssetTemplate { Template = AssetAttributePanel.Asset };
                    AssetTemplateService.Save(template);

                    if (!string.IsNullOrEmpty(RestoreFileName) && File.Exists(RestoreFileName))
                    {
                        try
                        {
                            File.Delete(this.RestoreFileName);
                        }
                        catch { }
                    }

                }
            }
            else
            {
                lblNoPermissions.Visible = true;
            }
            Response.Redirect(Request.Url.OriginalString);
        }
    }
}
