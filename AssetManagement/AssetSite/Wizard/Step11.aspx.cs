using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Batch;
using AppFramework.Core.ConstantsEnumerators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AssetSite.Helpers;
using Microsoft.Practices.Unity;
using AppFramework.Core.Classes.ScreensServices;

namespace AssetSite.Wizard
{
    public partial class Step11 : WizardController
    {
        [Dependency]
        public IBatchJobManager BatchJobManager { get; set; }
        [Dependency]
        public IAssetTypeTaxonomyManager AssetTypeTaxonomyManager { get; set; }
        [Dependency]
        public IBatchJobFactory BatchJobFactory { get; set; }
        [Dependency]
        public IScreensService ScreensService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AssetType == null)
                Response.Redirect("~/Wizard/Step1.aspx");

            if (!IsPostBack)
            {
                listAttributes.DataSource = 
                    AssetType
                    .Attributes
                    .Where(a => a.IsShownOnPanel)
                    .OrderBy(a => a.DisplayOrder);
                listAttributes.DataTextField = "NameLocalized";
                listAttributes.DataValueField = "UID";
                listAttributes.DataBind();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            (Master as MasterPageWizard).NextButton.Visible = false;
            (Master as MasterPageWizard).PreviousButton.Visible = false;
            (Master as MasterPageWizard).CancelButton.Visible = false;
        }

        protected void btnFinish_Click(object sender, EventArgs e)
        {
            AssetType.IsUnpublished = true;
            if (SaveCurrentAssetType())
            {
                SessionWrapper.CleanWizardSession();
                Response.Redirect("~/admin/");
            }
        }

        protected void Publish_Click(object sender, EventArgs e)
        {
            var isNew = AssetType.ID == 0;
            AssetType.IsUnpublished = false;
            if (SaveCurrentAssetType())
            {
                var atId = AssetType.ID;
                var atUid = AssetType.UID;
                SessionWrapper.CleanWizardSession();
                var job = BatchJobFactory.CreatePublishTypeJob(atId,
                        AuthenticationService.CurrentUserId,
                        ckNewRevision.Checked);
                BatchJobManager.SaveJob(job);

                if (isNew)
                {
                    var screens = ScreensService.GetScreensByAssetTypeUid(atUid);
                    if (!screens.Any())
                    {
                        Session[SessionVariables.AssetTypeWizard_BatchUrl] = job.NavigateUrl;
                        Response.Redirect(string.Format(
                            "~/admin/AdditionalScreens/AdditionalScreenStep1.aspx?atuid={0}",
                            atUid));
                    }
                }
                Response.Redirect(job.NavigateUrl);
            }
        }

        /// <summary>
        /// Saves the current asset type.
        /// </summary>
        private bool SaveCurrentAssetType()
        {
            var result = false;

            // check if autogeneration for Name field has set up properly
            if (_validateNameAutogeneration())
            {
                try
                {
                    var containers = Session[SessionVariables.AssetTypeWizard_Taxonomies] 
                        as List<TaxonomyContainer>;
                    AssetTypeRepository.Save(AssetType, AuthenticationService.CurrentUserId, containers);
                    result = true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    try
                    {
                        InitState(true);
                        using (var fs = File.Open(fileName, FileMode.Create))
                        {
                            AssetType.Serialize(fs);
                        }
                    }
                    catch (Exception inner)
                    {
                        Logger.Error(inner);
                        Debugger.Break();
                    }
                    throw;
                }
            }
            return result;
        }

        private bool _validateNameAutogeneration()
        {
            var nameGenerationAttributesCount = AssetType.Attributes.Count(a => a.IsUsedForNames);

            bool result = true;
            if (AssetType.AutoGenerateNameType != Enumerators.TypeAutoGenerateName.None &&
                nameGenerationAttributesCount == 0)
            {
                lblValidation.Visible = true;
                result = false;
            }
            return result;
        }
        
        protected override void btnPrevious_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Wizard/Step10.aspx");
        }
    }
}