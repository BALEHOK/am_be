using System;
using System.Linq;
using System.Web.UI.WebControls;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;

namespace AssetSite.Wizard
{
    public partial class Step2 : WizardController
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (AssetType == null)
            {
                Response.Redirect("~/Wizard/Step1.aspx");
            }

            if (!IsPostBack)
            {
                comboAssetTypeInheritance.DataSource = EntityType.GetAll();
                comboAssetTypeInheritance.DataTextField = "Name";
                comboAssetTypeInheritance.DataValueField = "ID";
                comboAssetTypeInheritance.DataBind();
                foreach (ListItem item in comboAssetTypeInheritance.Items)
                {
                    if (item.Value == AssetType.TypeId.ToString())
                    {
                        item.Selected = true;
                    }
                }

                chkActive.Checked = AssetType.IsActive;

                MeasureUnits.DataSource = AppFramework.Core.Classes.MeasureUnit.GetAll();
                MeasureUnits.DataBind();
                MeasureUnits.SelectedValue = AssetType.MeasureUnitId.ToString();

                IsInStock.Checked = AssetType.IsInStock;
                cbAllowBorrow.Checked = AssetType.AllowBorrow;
                cbParentChildRelations.Checked = AssetType.ParentChildRelations;
                ddlUseForNames.SelectedValue = AssetType.AutoGenerateName.ToString();
                cbContextIndexed.Checked = AssetType.IsContextIndexed;

                if (AssetType.IsInStock)
                    this.ClientScript.RegisterStartupScript(typeof(string), "s1",
                        "<script type=\"text/javascript\">$(function(){ShowMUSelect()});</script>");

                // current AT is inherited
                if (Session["BaseAssetTypeIdWizard"] != null || AssetType.BaseAssetTypeId != null)
                {
                    ctrlsInherit.Visible = true;
                    BindComboAssetTypes();
                    comboAssetTypes.SelectedValue
                        = Session["BaseAssetTypeIdWizard"] != null
                        ? Session["BaseAssetTypeIdWizard"].ToString()
                        : AssetType.BaseAssetTypeId.ToString();
                }
            }

            IsInStock.Attributes.Add("onclick", "ShowMUSelect()");

        }

        protected override void btnNext_Click(object sender, EventArgs e)
        {
            AssetType.AllowBorrow = cbAllowBorrow.Checked;
            AssetType.IsActive = chkActive.Checked;
            AssetType.IsInStock = IsInStock.Checked;
            AssetType.ParentChildRelations = cbParentChildRelations.Checked;
            AssetType.AutoGenerateName = int.Parse(ddlUseForNames.SelectedValue);
            AssetType.IsContextIndexed = cbContextIndexed.Checked;

            if (AssetType.IsInStock)
            {
                AssetType.MeasureUnitId = long.Parse(MeasureUnits.SelectedValue);
            }

            int type = int.Parse(comboAssetTypeInheritance.SelectedValue);
            AssetType.TypeId = type;
            long baseTypeId = 0;

            if (type == 2 && long.TryParse(comboAssetTypes.SelectedValue, out baseTypeId))
            {
                Session["BaseAssetTypeIdWizard"] = baseTypeId;
                var baseAT = AssetTypeRepository.GetById(baseTypeId);
                AssetType.CopyFrom(baseAT);
            }

            Response.Redirect("~/Wizard/Step3.aspx");
        }

        protected override void btnPrevious_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Wizard/Step1.aspx");
        }

        protected void comboAssetTypeInheritance_Selected(object sender, EventArgs e)
        {
            if (comboAssetTypeInheritance.SelectedIndex == 1)
            {
                ctrlsInherit.Visible = true;
                BindComboAssetTypes();
            }
            else
            {
                ctrlsInherit.Visible = false;
                Session["BaseAssetTypeIdWizard"] = null;
            }
        }

        private void BindComboAssetTypes()
        {
            comboAssetTypes.DataSource = AssetTypeRepository.GetAllPublished()
                .Where(a => a.ID != AssetType.ID);
            comboAssetTypes.DataValueField = "ID";
            comboAssetTypes.DataTextField = "Name";
            comboAssetTypes.DataBind();
            comboAssetTypes.Items.Insert(0, new ListItem());
        }
    }
}