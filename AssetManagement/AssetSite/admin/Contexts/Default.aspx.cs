using AppFramework.Core.Classes;
using AppFramework.DataProxy;
using System;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;

namespace AssetSite.admin.Contexts
{
    public partial class Default : BasePage
    {
        [Dependency]
        public IDataTypeService DataTypeService { get; set; }
        public Default(IUnitOfWork unitOfWork)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dropDataType.DataSource = DataTypeService.GetAll().Where(d => !d.IsInternal);
                dropDataType.DataTextField = "Name";
                dropDataType.DataValueField = "UID";
                dropDataType.DataBind();
                dropDataType.Items.Insert(0, new ListItem());
            }
        }


        protected void btnClose_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/admin/");
        }

        protected void OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            ContextsGrid.EditIndex = e.NewEditIndex;
        }

        protected void OnRowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            ContextsGrid.EditIndex = -1;
        }

        protected void OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            TextBox txtBox = ContextsGrid.Rows[e.RowIndex].Cells[0].FindControl("txtName") as TextBox;
            if (txtBox != null)
            {
                if (ContextsDataSource.UpdateParameters["Name"] == null)
                {
                    ContextsDataSource.UpdateParameters.Add("Name", TypeCode.String, txtBox.Text.Trim());
                }
            }
        }

        protected void ContextsDataSource_Updating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            if (e.InputParameters.Contains("ID"))
            {
                throw new NotImplementedException();
                //long id = long.Parse(e.InputParameters["ID"].ToString());
                //var entity = EntityContext.GetByID(id);
                //entity.Name = e.InputParameters["Name"].ToString();
                //e.InputParameters.Clear();
                //e.InputParameters.Add("item", entity);
            }
        }

        protected void btnAddContext_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
            //EntityContext entity = new EntityContext();
            //entity.Name = txtNewContext.Text.Trim();
            //entity.IsActive = true;
            //long dataTypeUid = 0;
            //if (long.TryParse(dropDataType.SelectedValue, out dataTypeUid))
            //{
            //    entity.DataTypeUid = dataTypeUid;
            //}
            //EntityContext.Save(entity);
            //Response.Redirect(Request.Url.OriginalString);
        }
    }
}
