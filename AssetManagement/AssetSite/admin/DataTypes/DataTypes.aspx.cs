using AppFramework.Core.Classes;
using AppFramework.DataProxy;
using System;
using System.Web.UI;
using Microsoft.Practices.Unity;

namespace AssetSite.admin
{
    public partial class DataTypes : BasePage
    {
        [Dependency]
        public IDataTypeService DataTypeService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DataTypesList.DataSource = DataTypeService.GetAll();
                DataTypesList.DataBind();
            }
        }
    }
}
