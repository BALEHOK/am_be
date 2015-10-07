using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.IE;
using System;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using AssetSite.Helpers;
using Microsoft.Practices.Unity;

namespace AssetSite.Wizard
{
    public partial class Step1 : WizardController
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool restore = !string.IsNullOrEmpty(Request.QueryString["Restore"]);
            bool saveExist = this.InitState(false);
            if (restore)
            {
                if (saveExist)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["Remove"]) &&
                        Request.QueryString["Remove"] == "true")
                    {
                        File.Delete(this.fileName);
                    }
                    else
                    {
                        using (FileStream fs = File.Open(this.fileName, FileMode.Open))
                        {
                            AssetType type = AssetType.Deserialize(fs);
                            this.AssetType = type;
                        }
                    }
                }
            }
            else
            {
                if (saveExist)
                {
                    this.RestoreSession.Visible = true;
                    this.CreationTime.Text = DateTime.Now.ToString();
                }
            }

            if (AssetType != null)
            {
                if (!IsPostBack)
                {
                    this.Name.Text = AssetType.NameInvariant;
                    this.Description.Text = AssetType.Comment;

                    if (Session["AssetTypeClass"] != null)
                    {
                        ddlAssetTypeClass.SelectedIndex = int.Parse(Session["AssetTypeClass"].ToString());
                        ddlAssetTypeClass.Visible = true;
                        ddlAssetTypeClass.Enabled = false;
                    }
                }

                Draft.Text = AssetType.IsActiveVersion && !AssetType.IsUnpublished ? "" : "Draft";
            }
            else
            {
                ddlAssetTypeClass.Visible = true;
            }

            lblAssetTypeClass.Visible = ddlAssetTypeClass.Visible;
        }

        protected override void btnNext_Click(object sender, EventArgs e)
        {
            if (AssetType == null)
            {
                if (ddlAssetTypeClass.SelectedValue == "0")
                {
                    AssetType = ImportExportManager.GetBasicAssetTypeConfiguration(
                        Enumerators.AssetTypeClass.NormalAssetType, UnitOfWork, LayoutRepository);
                }
                else if (ddlAssetTypeClass.SelectedValue == "1")
                {
                    AssetType = ImportExportManager.GetBasicAssetTypeConfiguration(
                        Enumerators.AssetTypeClass.DataAssetType, UnitOfWork, LayoutRepository);
                }
                else
                {
                    throw new ArgumentException("Unknown asset type class");
                }
                Session["AssetTypeClass"] = ddlAssetTypeClass.SelectedIndex;
            }

            string assetTypeName = this.Name.Text.Trim();
            AssetType.Name = assetTypeName;
            AssetType.Comment = this.Description.Text.Trim();
            Page.Validate();
            if (Page.IsValid)
            {
                Response.Redirect("~/Wizard/Step2.aspx");
            }
        }

        protected override void btnPrevious_Click(object sender, EventArgs e)
        {
            SessionWrapper.CleanWizardSession();
            Response.Redirect("~/admin/");
        }

        public string GetRevision()
        {
            if (this.AssetType != null)
            {
                if (this.AssetType.IsActiveVersion)
                    return (this.AssetType.Revision + 1).ToString();
                else
                    return this.AssetType.Revision.ToString();
            }
            else
            {
                return "1";
            }
        }

        protected void AssetTypeExistsValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (AssetType != null)
            {
                var validationResult = AssetTypeRepository.ValidateNameUniqueness(args.Value, AssetType.ID);
                var message = string.Join(", ", validationResult.ResultLines.Select(rule => rule.Message).ToArray());
                AssetTypeExistsValidator.ErrorMessage = message;
                args.IsValid = validationResult.IsValid;
            }
        }
    }
}
