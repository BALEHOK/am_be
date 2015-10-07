using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AssetSite.Helpers;

namespace AssetSite.Wizard
{
    public partial class EditAssetType : WizardController
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            (Master as MasterPageWizard).PreviousButton.Visible = false;
            (Master as MasterPageWizard).NextButton.Visible = false;
            (Master as MasterPageWizard).WizardMenu.Visible = false;

            if (ApplicationSettings.ApplicationType == ApplicationType.AssetManager)
            {
                int totalColumns = assetTypesGrid.Columns.Count;
                assetTypesGrid.Columns[totalColumns - 3].Visible = false;
                assetTypesGrid.Columns[totalColumns - 4].Visible = false;
            }

            assetTypesGrid.DataBind();
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            AssetType = null;
            Response.Redirect("~/admin/");
        }

        protected void assetTypesGrid_Edit(Object sender, GridViewEditEventArgs e)
        {
            e.Cancel = true;
            long assetTypeUID;
            if (long.TryParse(assetTypesGrid.DataKeys[e.NewEditIndex].Value.ToString(), out assetTypeUID))
            {
                SessionWrapper.CleanWizardSession();
                this.AssetType = AssetTypeRepository.GetByUid(assetTypeUID);

                // if selected asset type has draft, redirect to editing draft
                var draft = AssetTypeRepository.GetDraftById(AssetType.ID, AssetType.Revision);
                if (draft != null)
                    AssetType = draft;

                if (IsAssetTypeInBatch())
                {
                    ClientScript.RegisterStartupScript(this.GetType(),
                        "batchalert",
                        string.Format("<script>alert('Cannot edit {0} because it is currently in Batch system')</script>", AssetType.Name));
                }
                else
                {
                    Response.Redirect("~/Wizard/Step1.aspx");
                }
            }
        }

        private bool IsAssetTypeInBatch()
        {
            var batches = UnitOfWork.BatchJobRepository.AsQueryable();
            var actions = UnitOfWork.BatchActionRepository.AsQueryable();

            var runningActions = (from batch in batches
                                  join aa in actions
                                  on batch.BatchUid equals aa.BatchUid
                                  where aa.ActionType == (int)BatchActionType.PublishAssetType
                                      && (aa.Status == (short)BatchStatus.Created ||
                                          aa.Status == (short)BatchStatus.Waiting ||
                                          aa.Status == (short)BatchStatus.Running)
                                  select aa).ToList();

            List<long> assetTypeUids = new List<long>();
            foreach (var action in runningActions)
            {
                var parameters = XElement.Parse(action.ActionParams);
                assetTypeUids.AddRange(from p in parameters.Descendants("param")
                                       where p.Attribute("Key") != null && p.Attribute("Key").Value == "FromAssetType"
                                       select long.Parse(p.Attribute("Value").Value));
            }

            bool result = false;
            foreach (var uid in assetTypeUids)
            {
                var entity = UnitOfWork.DynEntityConfigRepository.Single(d => d.DynEntityConfigUid == uid);
                if (entity.DynEntityConfigId == AssetType.ID)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        protected void assetTypesGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ClientScriptManager cs = Page.ClientScript;
                var dec = e.Row.DataItem as AppFramework.Entities.DynEntityConfig;
                var deleteButton = e.Row.FindControl("btnDelete") as LinkButton;
                // disable deleting for deleted and system assettypes
                deleteButton.Visible = dec.Active && !PredefinedAttribute.IsPredefined(dec.DynEntityConfigId);
                string postbackEvent = cs.GetPostBackEventReference(deleteButton, "");
                deleteButton.OnClientClick = "return ShowConfirmationDialog(function(){ " + postbackEvent + " });";
            }
        }

        protected string ChopString(string originalString)
        {
            if (!string.IsNullOrEmpty(originalString) && originalString.Length > 20)
                return originalString.Substring(0, 20) + "...";
            else
                return string.Empty;
        }

        protected void DynEntityConfig_Deleting(object sender, EntityDataSourceChangingEventArgs args)
        {
            var entity = args.Entity as AppFramework.Entities.DynEntityConfig;
            var at = AssetTypeRepository.GetByUid(entity.DynEntityConfigUid);
            AssetTypeRepository.Delete(at);
            args.Cancel = true;
        }

        protected string GetTasksUrl(object atId)
        {
            return "../admin/Tasks/TasksList.aspx?AssetTypeId=" + atId.ToString();
        }

        protected string GetScreensUrl(object atUid)
        {
            return "../admin/AdditionalScreens/AdditionalScreenStep1.aspx?atuid=" + atUid.ToString();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            long atUid = long.Parse(((LinkButton)sender).CommandArgument);
            var at = AssetTypeRepository.GetByUid(atUid);
            AssetTypeRepository.Delete(at);
            assetTypesGrid.DataBind();
        }
    }
}