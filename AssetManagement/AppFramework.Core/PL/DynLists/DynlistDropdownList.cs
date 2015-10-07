using System;
using System.Linq;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.DynLists;

namespace AppFramework.Core.PL.DynLists
{
    public class DynlistDropdownList : DropDownList, IAssetAttributeControl
    {
        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {
            Attributes.Add(name, value);
        }

        public DynlistDropdownList()
        {
            this.AutoPostBack = true;
        }

        public DynlistDropdownList(AssetAttribute attribute)
            : this()
        {
            AssetTypeAttribute attrConf = attribute.GetConfiguration();

            var list = attrConf.DynamicList;

            if (!object.Equals(list, null))
            {
                this.Items.AddRange(list.Items.Select(it => new ListItem(it.Value, it.UID.ToString())).ToArray());
            }

            if (this.Items.FindByValue(attribute.ValueAsId.ToString()) != null)
                this.SelectedValue = attribute.ValueAsId.ToString();

            this.Enabled = attribute.GetConfiguration().DataType.Editable && this.Editable;

            AssetAttribute = attribute;
        }

        public DynlistDropdownList(DynamicListItem item)
            : this()
        {
            var assocList = item.AssociatedDynList;
            if (!object.Equals(assocList, null))
            {
                this.Items.Add(new ListItem("Select...", "-1"));
                this.Items.AddRange(assocList.Items.Select(it => new ListItem(it.Value, it.UID.ToString())).ToArray());
            }
        }

        #region IAssetAttributeControl Members

        public AssetAttribute GetAttribute()
        {
            return new AssetAttribute();
        }

        public bool Editable { get; set; }

        #endregion

        #region IBindableControl Members

        public void ExtractValues(System.Collections.Specialized.IOrderedDictionary dictionary)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
