using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using AppFramework.Core.Classes;

namespace AssetSite.admin.Taxonomies
{
    public partial class Categories : BaseController
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["Taxonomy"] = null;

            if (!IsPostBack)
            {
                GridDataBind();
            }
        }

        private void GridDataBind()
        {
            TaxonomiesGrid.DataSource = Taxonomy.GetAll(true);  // get categories, not taxonomies
            TaxonomiesGrid.DataBind();
        }

        protected void OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            TaxonomiesGrid.EditIndex = e.NewEditIndex;
            this.GridDataBind();
        }

        protected void OnRowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            TaxonomiesGrid.EditIndex = -1;
            this.GridDataBind();
        }

        protected void OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = TaxonomiesGrid.Rows[e.RowIndex];
            long uid = 0; // (long)TaxonomiesGrid.DataKeys[e.RowIndex].Value;

            long.TryParse((row.Cells[0].FindControl("UID") as HiddenField).Value, out uid);

            if (uid != 0)
            {
                Taxonomy t = Taxonomy.GetByUID(uid);
                if (t != null)
                {
                    t.Name = (row.Cells[1].Controls[0] as TextBox).Text;
                    t.Description = (row.Cells[2].Controls[0] as TextBox).Text;

                    t.Save();
                }

                TaxonomiesGrid.EditIndex = -1;
                this.GridDataBind();
            }
        }

        protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            GridViewRow row = TaxonomiesGrid.Rows[e.RowIndex];
            long uid = 0; // (long)TaxonomiesGrid.DataKeys[e.RowIndex].Value;

            long.TryParse((row.Cells[0].FindControl("UID") as HiddenField).Value, out uid);

            if (uid != 0)
            {
                Taxonomy t = Taxonomy.GetByUID(uid);
                if (t != null)
                {
                    t.Delete();
                }
            }

            this.GridDataBind();
        }

        protected void AddCategory(object sender, EventArgs e)
        {
            Taxonomy t = new Taxonomy()
            {
                Name = TaxText.Text,
                Description = TaxDescr.Text,
                IsCategory = true
            };
            t.Save();
            this.GridDataBind();

            TaxText.Text = string.Empty;
            TaxDescr.Text = string.Empty;
        }
    }
}
