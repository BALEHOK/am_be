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
    public partial class DynListDDHolder : UserControl, IAssetAttributeControl
    {
        [Dependency]
        public IDynListItemService DynListItemService { get; set; }

        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {        
            // TODO: empty method?
        }

        private List<SavedState> _states;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_states == null)
            {
                // if it's a first load - init state with first dynamic list (from assettype attribute)
                _states = new List<SavedState>();
                if (AssetAttribute.DynamicListValues.Count != 0)
                {
                    _states.AddRange(
                        AssetAttribute.DynamicListValues.OrderBy(d => d.DisplayOrder).Select(v =>
                            new SavedState
                            {
                                ListId = v.DynamicListUid,
                                ItemId = v.DynamicListItemUid,
                                Value = new TranslatableString(v.Value).GetTranslation()
                            })
                    );

                    for (int i = 0; i < _states.Count; i++)
                        _states[i].Index = i;
                }
                else
                {
                    var list = AssetAttribute.GetConfiguration().DynamicListUid;
                    if (list.HasValue)
                    {
                        _states.Add(new SavedState { ListId = list.Value, ItemId = 0, Index = 0 });
                    }
                    else
                    {
                        Controls.Add(new Literal { Text = "Dynamic list not found, please contact administrator" });
                        return;
                    }
                }
            }

            repDropDownLists.DataSource = _states;
            repDropDownLists.DataBind();
        }

        protected void OnChildSelectionChanged(object sender, EventArgs e)
        {
            var ddl = sender as DynListDropDown;
            _states[ddl.StateIndex].ItemId = ddl.SelectedValue;
            _states[ddl.StateIndex].Value = ddl.SelectedText;

            if (ddl.StateIndex != _states.Count - 1)
            {
                int toDelete = (_states.Count - 1) - ddl.StateIndex;
                for (int i = 0; i < toDelete; i++)
                {
                    _states.RemoveAt(_states.Count - 1); // remove from stored list
                }
            }

            if (ddl.SelectedValue > 0)
            {
                var item = DynListItemService.GetByUid(ddl.SelectedValue);
                if (item != null && item.AssociatedDynList != null)
                {
                    int lstIndex = _states.Last().Index;
                    _states.Add(new SavedState()
                    {
                        Value = ddl.SelectedText,
                        ItemId = ddl.SelectedValue,
                        ListId = item.AssociatedDynList.UID,
                        Index = lstIndex + 1
                    });
                }
            }

            repDropDownLists.DataSource = _states;
            repDropDownLists.DataBind();
            updatePanel.Update();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            repDropDownLists.DataSource = null;
            repDropDownLists.DataBind();

            GridViewRow li = this.NamingContainer as GridViewRow;
            AppFramework.Core.Classes.Asset a = li.DataItem as AppFramework.Core.Classes.Asset;

            if (a == null)
            {
                throw new NullReferenceException("Cannot bind asset to AssetsGrid row");
            }

            Object value = DataBinder.Eval(li.DataItem, string.Format("[{0}].Value", AssetAttribute.GetConfiguration().DBTableFieldName));
            if (value != null)
            {
                Literal dd = new Literal();
                if (value != null) dd.Text = new TranslatableString(value.ToString()).GetTranslation();
                DynListDDControls.Controls.Clear();
                DynListDDControls.Controls.Add(dd);
            }
        }

        public bool IsEditable()
        {
            return this.Editable;
        }

        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresControlState(this);    // register this control as control needed to save its state
            base.OnInit(e);
        }

        protected override object SaveControlState()
        {
            return _states;
        }

        protected override void LoadControlState(object savedState)
        {
            _states = savedState as List<SavedState>;
        }

        public AssetAttribute GetAttribute()
        {
            AssetAttribute attr = AssetAttribute;
            attr.DynamicListValues.Clear();

            long parentId = 0; // needed to restore order of DropDownList when viewing and editing

            // each state from saved states copy in attribute DynamicListValues
            foreach (SavedState state in _states)
            {
                long ListId = state.ListId, ItemId = state.ItemId;
                string selectedText = state.Value;

                if (ListId == 0 || ItemId == 0 || string.IsNullOrEmpty(selectedText))
                    continue;

                DynamicListValue v = new DynamicListValue()
                {
                    ParentListId = parentId,
                    DynamicListUid = ListId,
                    DynamicListItemUid = ItemId,
                    AssetAttribute = attr,
                    DisplayOrder = attr.DynamicListValues.Count,
                    Value = (selectedText == AppFramework.Core.Properties.Resources.SelectText ? string.Empty : selectedText)
                };
                attr.AddDynamicListValue(v);
                parentId = ListId;
            }

            // actually, value don't needed for attribute, all info stored in DynamicListValues
            attr.Value = string.Join(" ", AssetAttribute.DynamicListValues.Select(dlv => dlv.Value));
            return attr;
        }

        public bool Editable { get; set; }
    }

    [Serializable]
    public class SavedState
    {
        public long ListId { get; set; }
        public long ItemId { get; set; }
        public string Value { get; set; }
        public int Index { get; set; }
    }
}