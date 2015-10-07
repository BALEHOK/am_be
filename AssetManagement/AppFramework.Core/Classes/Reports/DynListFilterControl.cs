using System.Web.UI.WebControls;

namespace AppFramework.Core.Classes.Reports
{
    public class DynListFilterControl : DropDownList, IFilterControl
    {
        public DynListFilterControl(ReportField field)
        {
            AssetTypeAttribute attr = field.GetTypeAttribute();
            if (attr != null && attr.IsDynListValue && attr.DynamicList != null)
            {
                this.DataSource = attr.DynamicList.Items;
                this.DataBind();
            }
        }

        public override string DataTextField
        {
            get
            {
                return "Value";
            }
        }

        public override string DataValueField
        {
            get
            {
                return "UID";
            }
        }

        #region IFilterControl Members

        public string GetValue()
        {
            return this.SelectedValue;
        }

        public string GetText()
        {
            return this.SelectedItem.Text;
        }

        #endregion
    }
}
