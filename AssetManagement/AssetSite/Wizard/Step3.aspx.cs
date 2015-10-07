using System;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;

namespace AssetSite.Wizard
{
    public partial class Step3 : WizardController
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (AssetType == null)
            {
                Response.Redirect("~/Wizard/Step1.aspx");
            }

            BindData();
        }

        private void BindData()
        {
            GridAttributes.RowDataBound += GridAttributes_RowDataBound;
            GridAttributes.DataSource = AssetType.AllAttributes.Where(a => a.IsShownOnPanel).OrderBy(a => a.DisplayOrder);
            GridAttributes.DataBind();
        }

        protected void GridAttributes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            var attr = e.Row.DataItem as AssetTypeAttribute;
            if (!attr.AllowEditConfig)
            {
                e.Row.FindControl("btnDelete").Visible = false;
                e.Row.FindControl("linkEdit").Visible = false;                
            }

            if (attr.IsRequired)
            {
                e.Row.Cells[0].Font.Bold = true;
            }

            if (!attr.IsActive)
            {
                e.Row.Cells[0].ForeColor = Color.Gray;
            }
        }

        protected override void btnNext_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Wizard/Step4.aspx");
        }

        protected override void btnPrevious_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Wizard/Step2.aspx");
        }

        protected void btnNewAttribute_Click(object sender, EventArgs e)
        {

        }

        protected void GridAttributes_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridAttributes.EditIndex = e.NewEditIndex;
            BindData();
        }

        protected void GridAttributes_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridAttributes.EditIndex = -1;
            BindData();
        }

        protected void GridAttributes_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridAttributes.EditIndex = -1;
            BindData();
        }

        protected void GridAttributes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            long uid = (long)GridAttributes.DataKeys[e.RowIndex].Value;
            AssetType.RemoveAttribute(uid);
            //assetType.Attributes.RemoveAll(a => a.UID == uid);
            GridAttributes.EditIndex = -1;
            BindData();
        }

        protected void GridAttributes_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {

        }

        protected string ChopString(string originalString)
        {
            if (!string.IsNullOrEmpty(originalString) && originalString.Length > 50)
                return originalString.Substring(0, 50) + "...";
            else
                return string.Empty;
        }

        protected void GridAttributes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null || string.IsNullOrEmpty(e.CommandArgument.ToString()))
                return;

            long uid = Convert.ToInt64(e.CommandArgument);
            var sortAttributes = AssetType.Attributes.Where(a => a.IsShownOnPanel).OrderBy(a => a.DisplayOrder).ToList();
            var item = sortAttributes.FirstOrDefault(a => a.UID == uid);
            int index = sortAttributes.IndexOf(item);
            if (e.CommandName == "down" && index + 1 != sortAttributes.Count)
            {
                int temp = sortAttributes[index].DisplayOrder;
                sortAttributes[index].DisplayOrder = sortAttributes[index + 1].DisplayOrder;
                sortAttributes[index + 1].DisplayOrder = temp;
            }
            else if (e.CommandName == "up" && index != 0)
            {
                int temp = sortAttributes[index].DisplayOrder;
                sortAttributes[index].DisplayOrder = sortAttributes[index - 1].DisplayOrder;
                sortAttributes[index - 1].DisplayOrder = temp;
            }
            BindData();
        }
    }
}
