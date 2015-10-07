using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using AppFramework.Core.Classes;
using System.Web.UI.WebControls;

namespace AppFramework.Core.PL
{
    public class CheckboxControl : CheckBox, IAssetAttributeControl
    {
        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {
            Attributes.Add(name, value);
        }

        public CheckboxControl(AssetAttribute attribute)
        {
            AssetAttribute = attribute;
        }

        protected override void OnInit(EventArgs e)
        {
            this.Enabled = Editable;
            bool val = false;
            bool.TryParse(AssetAttribute.Value, out val);
            this.Checked = val;
            base.OnInit(e);
        }

        /// <summary>
        /// In case if control will be used in grid
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            GridViewRow li = this.NamingContainer as GridViewRow;
            Asset a = li.DataItem as Asset;

            if (a == null)
            {
                throw new NullReferenceException("Cannot bind asset to AssetsGrid row");
            }

            Object value = DataBinder.Eval(li.DataItem, string.Format("[{0}].Value", AssetAttribute.GetConfiguration().DBTableFieldName));
            if (value != null)
            {
                bool val = false;
                bool.TryParse(value.ToString(), out val);
                this.Checked = val;
            }
        }

        #region IAssetAttributeControl Members

        public AppFramework.Core.Classes.AssetAttribute GetAttribute()
        {
            AssetAttribute.Value = this.Checked.ToString();
            return AssetAttribute;
        }

        public bool Editable { get; set; }

        #endregion
    }
}
