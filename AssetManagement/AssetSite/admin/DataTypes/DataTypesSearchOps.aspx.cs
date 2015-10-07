using AppFramework.Core.Classes;
using AppFramework.Core.DataTypes;
using AppFramework.DataProxy;
using AppFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;

namespace AssetSite.admin
{
    public partial class DataTypesSearchOps : BasePage
    {
        [Dependency]
        public IDataTypeService DataTypeService { get; set; }
        private CustomDataType _dataType;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["Id"]))
                Response.Redirect("~/admin/DataTypes/DataTypes.aspx");

            _dataType = DataTypeService.GetByUid(long.Parse(Request.QueryString["Id"]));
            UnitOfWork.DataTypeRepository.LoadProperty(_dataType.Base, dt => dt.SearchOperators);
        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            var assignedOperators = new List<long>();
            foreach (GridViewRow row in OperatorsList.Rows)
            {
                CheckBox selectedControl = row.Cells[0].FindControl("Selected") as CheckBox;
                HiddenField uidControl = row.Cells[0].FindControl("UID") as HiddenField;

                if (selectedControl != null && uidControl != null)
                {
                    if (selectedControl.Checked)
                    {
                        long uid = long.Parse(uidControl.Value);
                        assignedOperators.Add(uid);
                    }
                }
            }

            DataTypeService.AssignSearchOperators(_dataType, assignedOperators);
            Response.Redirect("DataTypes.aspx");
        }

        protected void OperatorsList_DataBinding(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var @operator = e.Row.DataItem as SearchOperators;
                CheckBox selected = e.Row.Cells[0].FindControl("Selected") as CheckBox;
                if (selected != null && @operator != null)
                {
                    selected.Checked = _dataType.Base.SearchOperators.Any(s => s.SearchOperatorUid == @operator.SearchOperatorUid);
                }
            }
        }
    }
}
