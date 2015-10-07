using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;

namespace AppFramework.Core.PL
{
    public class PermissionsControl : UserControl, IAssetAttributeControl
    {
        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {
            _dropdown.Attributes.Add(name, value);
        }

        private readonly DropDownList _dropdown = new DropDownList { ID = "dropdownList" };
        private List<ListItem> Items
        {
            get
            {
                return _items ?? (_items = new List<ListItem>
                {
                    new ListItem(Permission.DDDD.GetFriendlyName(), Permission.DDDD.GetCode().ToString()),
                    new ListItem(Permission.RDDD.GetFriendlyName(), Permission.RDDD.GetCode().ToString()),
                    new ListItem(Permission.RWDD.GetFriendlyName(), Permission.RWDD.GetCode().ToString()),
                    new ListItem(Permission.RWRD.GetFriendlyName(), Permission.RWRD.GetCode().ToString()),
                    new ListItem(Permission.RWRW.GetFriendlyName(), Permission.RWRW.GetCode().ToString())
                });
            }
        }
        private List<ListItem> _items;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Controls.Clear();
            if (Editable)
            {
                InitDropdown();
                Controls.Add(_dropdown);
            }
            else
            {
                Controls.Add(GetLabel(AssetAttribute.Value));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Editable)
            {
                Page.ClientScript.RegisterClientScriptInclude("image-combo", "/javascript/jquery.dd.js");
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_ComboInitalize_" + this._dropdown,
                    "$(function () { $('#" + _dropdown.ClientID + "').msDropDown(); });", true);
            }
        }

        public PermissionsControl(AssetAttribute attribute)
        {
            AssetAttribute = attribute;
        }

        private WebControl GetLabel(string permission)
        {
            if (String.IsNullOrEmpty(permission))
                permission = "0";

            var item = Items.SingleOrDefault(i => i.Value == permission);
            var image = new Image()
            {
                ImageUrl = string.Format("/images/permissions/{0}.png", permission),
                AlternateText = item.Text,
                ToolTip = item.Text
            };
            return image;
        }

        private void InitDropdown()
        {
            _dropdown.Items.AddRange(Items.ToArray());
            foreach (ListItem item in _dropdown.Items)
            {
                item.Attributes.Add("title", string.Format("/images/permissions/{0}.png", item.Value));
                if (item.Value == AssetAttribute.Value)
                {
                    item.Selected = true;
                }
            }
        }

        #region IAssetAttributeControl Members

        public AppFramework.Core.Classes.AssetAttribute GetAttribute()
        {
            if (Editable)
            {
                AssetAttribute.Value = _dropdown.SelectedValue;
            }
            return AssetAttribute;
        }

        public bool Editable { get; set; }

        #endregion

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
                this.Controls.Clear();
                this.Controls.Add(GetLabel(value.ToString()));
            }
        }
    }
}
