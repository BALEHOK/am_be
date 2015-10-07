namespace AssetSite.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;
    using AppFramework.Core.Classes.DynLists;

    public partial class DynListEditDialog : UserControl
    {
        public List<DynamicList> DynamicLists { get; set; }
        public DynamicList DynamicList { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager
                .GetCurrent(Page)
                .RegisterAsyncPostBackControl(LinkButton1);
            if (DynamicList == null)
                return;
            lbtnAddItem.Attributes.Add("onclick",
                        "return OnAdd(" +
                            DynamicList.UID + ",'" +
                            tbDynListItemName.ClientID + "','" +
                            dialog.ClientID + "','" +
                            cbAssociated.ClientID + "','" +
                            lstAssociatedLists.ClientID + "');");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "_DlgInitalize_" + dialog.ClientID,
                @"$(function () { 
                    $('#" + dialog.ClientID + "').dialog({ autoOpen: false, width: 420, height: 520, close: function(event, ui) { __doPostBack('" +
                LinkButton1.UniqueID + "', ''); } });});", true);

            lstAssociatedLists.Attributes.Add("style", "display:none;");
            cbAssociated.Attributes.Add("onclick", "$('#" + lstAssociatedLists.ClientID + "').toggle();");
            editBtn.Attributes["onclick"] = "return ShowDialog('" + dialog.ClientID + "');";
            dialog.Attributes["title"] = Resources.Global.ItemsManagement; 

            DynListItems.Controls.Clear();
            foreach (var item in DynamicList.Items)
            {
                var editCtrl = (DynListItemEdit)Page.LoadControl("~/Controls/DynListItemEdit.ascx");
                editCtrl.DynListUID = this.DynamicList.UID;
                editCtrl.DynListItemValue = item.Value;
                editCtrl.DynListItemId = item.UID;
                DynListItems.Controls.Add(editCtrl);
            }

            // dropdown for possibly associated lists
            BindAssociatedLists();
        }

        private void BindAssociatedLists()
        {
            lstAssociatedLists.DataSource = DynamicLists.Where(it => it.UID != DynamicList.UID);
            lstAssociatedLists.DataBind();
        }
    }
}