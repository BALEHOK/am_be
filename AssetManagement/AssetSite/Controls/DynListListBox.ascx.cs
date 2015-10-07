using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.DynLists;
using AppFramework.Core.PL;
using Microsoft.Practices.Unity;

namespace AssetSite.Controls
{
    public partial class DynListListBox : UserControl, IAssetAttributeControl
    {
        [Dependency]
        public IDynamicListsService DynamicListsService { get; set; }

        private bool _editable;

        public AssetAttribute AssetAttribute
        {
            get;
            set;
        }

        public void AddAttribute(string name, string value)
        {            
        }

        public bool Bindable
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Editable)
                {
                    if (AssetAttribute.DynamicListValues.Count != 0)
                    {
                        foreach (DynamicListValue item in AssetAttribute.DynamicListValues)
                        {
                            ListItem itm = ListItems.Items.FindByText(new TranslatableString(item.Value).GetTranslation());
                            if (itm != null) itm.Selected = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Displays as text.
        /// </summary>
        private void DisplayAsText(List<DynamicListValue> values)
        {
            Literal lit = new Literal();
            lit.Text = string.Join("<br />",
                values.OrderBy(v => v.DisplayOrder)
                .Select(vl => new TranslatableString(vl.Value).GetTranslation()).ToArray());
            DispControls.Controls.Add(lit);
        }

        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresControlState(this);
            this.DispControls.Controls.Clear();
            if (AssetAttribute.GetConfiguration().DynamicListUid.HasValue)
            {
                long uid = AssetAttribute.GetConfiguration().DynamicListUid.Value;
                var list = DynamicListsService.GetByUid(uid);
                ListItems.Visible = false;
                ItemText.Visible = false;
                if (Editable)
                {
                    ListItems.DataSource = list.Items;
                    ListItems.DataTextField = "Value";
                    ListItems.DataValueField = "UID";
                    ListItems.DataBind();
                    ListItems.Visible = true;
                }
                else
                {
                    if (!this.Bindable) // if not binging in grid - display it
                        this.DisplayAsText(AssetAttribute.DynamicListValues);
                }
            }

            base.OnInit(e);
        }

        protected override void OnDataBinding(EventArgs e)
        {
            GridViewRow li = this.NamingContainer as GridViewRow;
            AppFramework.Core.Classes.Asset a = li.DataItem as AppFramework.Core.Classes.Asset;
            this.DispControls.Controls.Clear();
            if (a == null)
            {
                throw new NullReferenceException("Cannot bind asset to AssetsGrid row");
            }

            AppFramework.Core.Classes.Asset ast = li.DataItem as AppFramework.Core.Classes.Asset;
            if (ast != null)
                this.DisplayAsText(ast[AssetAttribute.GetConfiguration().DBTableFieldName].DynamicListValues);

            base.OnDataBinding(e);
        }

        #region IAssetAttributeControl Members

        public AssetAttribute GetAttribute()
        {
            List<DynamicListValue> values = new List<DynamicListValue>();
            long ListId = AssetAttribute.GetConfiguration().DynamicListUid.Value;
            foreach (int idx in ListItems.GetSelectedIndices())
            {
                ListItem item = ListItems.Items[idx];
                DynamicListValue v = new DynamicListValue()
                {
                    DynamicListUid = ListId,
                    DynamicListItemUid = long.Parse(item.Value),
                    AssetAttribute = AssetAttribute,
                    Value = new TranslatableString(item.Text).GetTranslation()
                };
                values.Add(v);
            }
            AssetAttribute.DynamicListValues.Clear();
            AssetAttribute.DynamicListValues.AddRange(values);
            AssetAttribute.Value = "0";
            return AssetAttribute;
        }

        public bool Editable
        {
            get
            {
                return _editable;
            }
            set
            {
                _editable = value;
            }
        }

        #endregion

        #region IBindableControl Members

        public void ExtractValues(System.Collections.Specialized.IOrderedDictionary dictionary)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}