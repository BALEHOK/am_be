using System;
using System.Linq;
using System.Web.UI.WebControls;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;


namespace AssetSite.Wizard
{
    public partial class Step9 : WizardController
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AssetType == null)
            {
                Response.Redirect("~/Wizard/Step1.aspx");
            }

            gridAttributes.DataSource = AssetType.Attributes.Where(a => a.IsShownOnPanel).OrderBy(a => a.DisplayOrder);
            gridAttributes.DataBind();
        }

        private void UpdateAttributes()
        {
            foreach (GridViewRow row in gridAttributes.Rows)
            {
                int id;
                string val = (row.Cells[0].FindControl("UID") as HiddenField).Value;

                if (int.TryParse(val, out id))
                {
                    AssetTypeAttribute attrib = AssetType.Attributes.Single(g => g.UID == id);
                    CheckBox cb = row.Cells[2].FindControl("CheckBoxKeyword") as CheckBox;
                    if (cb != null)
                        attrib.IsKeyword = cb.Checked;
                    cb = row.Cells[3].FindControl("CheckBoxIndex") as CheckBox;
                    if (cb != null)
                        attrib.IsFullIndex = cb.Checked;
                    cb = row.Cells[3].FindControl("CheckBoxDescription") as CheckBox;
                    if (cb != null)
                        attrib.IsDescription = cb.Checked;
                    cb = row.Cells[3].FindControl("CheckBoxDisplayOnResultList") as CheckBox;
                    if (cb != null)
                        attrib.DisplayOnResultList = cb.Checked;
                    cb = row.Cells[3].FindControl("CheckBoxDisplayOnExtResultList") as CheckBox;
                    if (cb != null)
                        attrib.DisplayOnExtResultList = cb.Checked;
                }

            }
        }

        protected override void btnNext_Click(object sender, EventArgs e)
        {
            UpdateAttributes();
            Response.Redirect("~/Wizard/Step10.aspx");
        }

        protected override void btnPrevious_Click(object sender, EventArgs e)
        {
            UpdateAttributes();
            Response.Redirect("~/Wizard/Step4.aspx");
        }
    }
}
