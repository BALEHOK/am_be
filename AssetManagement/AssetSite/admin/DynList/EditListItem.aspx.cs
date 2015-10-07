using System;
using System.Linq;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.DynLists;
using Microsoft.Practices.Unity;

namespace AssetSite.DynList
{
    public partial class EditListItem : BasePage
    {
        [Dependency]
        public IDynListItemService DynListItemService { get; set; }
        [Dependency]
        public IDynamicListsService DynamicListsService { get; set; }

        DynamicList _dynList;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["Id"]))
            {
                _dynList = DynamicListsService.GetByUid(long.Parse(Request.QueryString["Id"]));
                if (object.Equals(_dynList, null))
                    Response.Redirect("~/admin/DynList/");
            }
            else
            {
                Response.Redirect("~/admin/DynList/");
            }

            if (!IsPostBack)
            {
                AssocList.Attributes.CssStyle["display"] = "none";
                DList.DataSource = DynamicListsService.GetAll().Where(it => it.UID != _dynList.UID);
                DList.DataBind();
                long itemid;
                if (!string.IsNullOrEmpty(Request.QueryString["itemId"]) && long.TryParse(Request.QueryString["itemId"], out itemid))
                {
                    var item = _dynList.Items.FirstOrDefault(di => di.ID == itemid);
                    if (item != null)
                    {
                        NewValue.Text = item.OriginalValue;
                        if (item.AssociatedDynList != null)
                        {
                            IsAssoc.Checked = true;
                            AssocList.Attributes.CssStyle["display"] = "block";
                            DList.SelectedValue = item.AssociatedDynList.UID.ToString();
                        }
                    }
                    else
                    {
                        Response.Redirect(string.Format("~/admin/DynList/ListItems.aspx?Id={0}", _dynList.UID));
                    }
                }              
            }

            IsAssoc.Attributes.Add("onclick", "switchAssoc()");
        }

        protected void AddItem(object sender, EventArgs e)
        {
            long itemid;
            if (!string.IsNullOrEmpty(Request.QueryString["itemId"]) && long.TryParse(Request.QueryString["itemId"], out itemid))
            {
                var item = _dynList.Items.FirstOrDefault(di => di.ID == itemid);
                if (item != null)
                {
                    item.Value = NewValue.Text;
                    if (IsAssoc.Checked)
                    {
                        long uid = 0;
                        if (long.TryParse(DList.SelectedValue, out uid) && uid != 0)
                        {
                            item.AssociatedDynList = DynamicListsService.GetByUid(uid);
                        }
                    }
                    else
                    {
                        item.AssociatedDynList = null;
                    }
                    DynListItemService.Update(item.Base);
                }
            }
            else
            {
                var item = new DynamicListItem()
                {
                    ActiveVersion = true,
                    ParentDynList = _dynList,
                    DisplayOrder = (_dynList.Items.Count != 0) ? _dynList.Items.Max(i => i.DisplayOrder) + 1 : 1,
                    Value = NewValue.Text
                };

                if (IsAssoc.Checked)
                {
                    long uid = 0;
                    if (long.TryParse(DList.SelectedValue, out uid) && uid != 0)
                    {
                        item.AssociatedDynList = DynamicListsService.GetByUid(uid);
                    }
                }
                _dynList.Items.Add(item);
                DynamicListsService.Save(_dynList.Base);
            }

            Response.Redirect(string.Format("~/admin/DynList/ListItems.aspx?Id={0}", _dynList.UID));
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("~/admin/DynList/ListItems.aspx?Id={0}", _dynList.UID));
        }
    }
}