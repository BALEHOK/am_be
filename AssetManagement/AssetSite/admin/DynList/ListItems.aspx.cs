using System;
using System.Linq;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.DynLists;
using Microsoft.Practices.Unity;

namespace AssetSite.DynList
{
    public partial class ListItems : BasePage
    {
        [Dependency]
        public IDynListItemService DynListItemService { get; set; }
        [Dependency]
        public IDynamicListsService DynamicListsService { get; set; }

        DynamicList _dynList;

        protected void Page_Load(object sender, EventArgs e)
        {
            Save.Attributes.Add("onclick", string.Format("getArray('{0}')", SortOrder.ClientID));
            if (!string.IsNullOrEmpty(Request.QueryString["Id"]))
            {
                _dynList = DynamicListsService.GetByUid(long.Parse(Request.QueryString["Id"]));
                if (object.Equals(_dynList, null))
                    Response.Redirect("~/admin/DynList/");
            }
            else
                Response.Redirect("~/admin/DynList/");
            AddNewUrl.NavigateUrl = string.Format("EditListItem.aspx?Id={0}", _dynList.UID);

            if (!IsPostBack)
            {
                ItemsList.DataSource = _dynList.Items.OrderBy(i => i.DisplayOrder);
                ItemsList.DataBind();
            }

            Save.Attributes.Add("onclick", string.Format("arr('{0}','{1}')", ItemsList.ClientID, SortOrder.ClientID));
            DelSel.Attributes.Add("onclick", string.Format("SelForDelete('{0}','{1}')", ItemsList.ClientID, DelIds.ClientID));

        }

        protected void Save_Click(object sender, EventArgs e)
        {
            UpdateDisplayOrder();
            DynamicListsService.Save(_dynList.Base);
            Response.Redirect("~/admin/DynList/");
        }

        /// <summary>
        /// Updates the display order.
        /// </summary>
        private void UpdateDisplayOrder()
        {
            if (!string.IsNullOrEmpty(SortOrder.Value))
            {
                try
                {
                    long[] ord = SortOrder.Value.Split(',').Select(s => long.Parse(s)).ToArray();
                    for (int i = 0; i < ord.Length; i++)
                    {
                        _dynList.Items.First(it => it.UID == ord[i]).DisplayOrder = i;
                    }
                }
                catch   // if exception caught either ids not in correct format, either item with this id not exist
                {
                    // just ignore, nothing changes
                }
            }
        }

        protected void DelSelClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DelIds.Value))
            {
                try
                {
                    long[] ord = DelIds.Value.Split(',').Select(s => long.Parse(s)).ToArray();
                    for (int i = 0; i < ord.Length; i++)
                    {
                        DynamicListItem item = _dynList.Items.Single(it => it.UID == ord[i]);
                        DynListItemService.Delete(item.Base);
                    }
                }
                catch   // if exception caught either ids not in correct format, either item with this id not exist
                {
                    // just ignore, nothing changes
                }
            }
            Response.Redirect(Request.Url.OriginalString);
        }

        protected void linkEditItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ItemsList.SelectedValue) && _dynList != null)
            {
                Response.Redirect(string.Format("~/admin/DynList/EditListItem.aspx?Id={0}&itemId={1}", _dynList.UID, ItemsList.SelectedValue));
            }
        }
    }
}
