using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;

namespace AssetSite.Wizard
{
    public partial class Step4 : WizardController
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AssetType == null)
            {
                Response.Redirect("~/Wizard/Step1.aspx");
            }

            Page.Form.Attributes.Add("onsubmit", "return SaveSortPanelItem(this)");
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_DlgInitalize_" + this.ClientID,
                "$(function () { $('#Step4DialogContainer').dialog({ autoOpen: false, width: 350, height: 420 }); });", true);
            
            BindData();
        }

        private void BindData()
        {
            GridAttributes.DataSource = AssetType.Attributes.Where(a => a.IsShownOnPanel).OrderBy(a => a.DisplayOrder);
            GridAttributes.DataBind();

            if (AssetType.AutoGenerateNameType == Enumerators.TypeAutoGenerateName.InsertOnly ||
                AssetType.AutoGenerateNameType == Enumerators.TypeAutoGenerateName.InsertUpdate)
            {
                lstAttribs.DataSource = null;
                var sortInfo = AssetType.Attributes.Where(a => a.IsUsedForNames).ToList();
                if (sortInfo.Count > 0)
                {
                    var noOrder = sortInfo.Where(i => !i.NameGenOrder.HasValue);
                    var ordered = sortInfo.Where(i => i.NameGenOrder.HasValue).OrderBy(i => i.NameGenOrder.Value);

                    lstAttribs.DataSource = ordered.Union(noOrder);
                }
                lstAttribs.DataBind();
            }
        }

        protected void cbUseForNames_OnCheckedChanged(object sender, EventArgs e)
        {
            UpdateAttributes();
            BindData();
        }

        private void UpdateAttributes()
        {
            foreach (GridViewRow row in GridAttributes.Rows)
            {
                int uid;
                string val = (row.Cells[0].FindControl("UID") as HiddenField).Value;

                if (int.TryParse(val, out uid))
                {
                    AssetTypeAttribute attrib = AssetType.Attributes.Single(g => g.UID == uid);
                    
                    var cbShowInGrid = row.Cells[3].FindControl("chkShow") as CheckBox;
                    if (cbShowInGrid != null)
                        attrib.IsShownInGrid = cbShowInGrid.Checked;

                    var cbUseForNames = row.Cells[4].FindControl("cbUseForNames") as CheckBox;
                    if (cbUseForNames != null)
                        attrib.IsUsedForNames = cbUseForNames.Checked;
                }
            }
        }

        private void GenerateAutonamingFormula()
        {
            var sortAttribs = AssetType.Attributes.Where(a => a.IsUsedForNames).ToList();
            var noOrder = sortAttribs.Where(a => !a.NameGenOrder.HasValue);
            var ordered = sortAttribs.Where(a => a.NameGenOrder.HasValue)
                                     .OrderBy(a => a.NameGenOrder != null ? a.NameGenOrder.Value : 0);
            var finalOrder = ordered.Union(noOrder);
        }

        protected override void btnNext_Click(object sender, EventArgs e)
        {
            UpdateAttributes();
            if (AssetType.AutoGenerateNameType != Enumerators.TypeAutoGenerateName.None &&
                AssetType.Attributes.All(a => !a.IsUsedForNames))
            {
                // no one attribute selected for name auto-generation
                lblValidation.Visible = true;
            }
            else
            {                
                Response.Redirect("~/Wizard/Step9.aspx");
            }
        }

        protected override void btnPrevious_Click(object sender, EventArgs e)
        {
            UpdateAttributes();
            Response.Redirect("~/Wizard/Step3.aspx", true);
        }

        public bool IsNameAutogenEnabled()
        {
            return this.AssetType.AutoGenerateNameType == Enumerators.TypeAutoGenerateName.InsertOnly ||
                this.AssetType.AutoGenerateNameType == Enumerators.TypeAutoGenerateName.InsertUpdate;
        }

        [WebMethod]
        public static void SaveNameGenOrder(string inputData)
        {
            string[] pairs = inputData.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (pairs.Length == 0) return;

            var assetType = HttpContext.Current.Session["AssetTypeWizard"] as AssetType;

            foreach (string pair in pairs)
            {
                string[] vals = pair.Trim(new char[2] { '(', ')' }).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                long attribId = long.Parse(vals[0]);
                int order = int.Parse(vals[1]);

                for (int i = 0; i < assetType.Attributes.Count; i++)
                {
                    if (assetType.Attributes[i].ID == attribId)
                    {
                        assetType.Attributes[i].NameGenOrder = order;
                        break;
                    }
                }
            }

            HttpContext.Current.Session["AssetTypeWizard"] = assetType;
        }
    }
}