using System;
using System.Linq;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.ScreensServices;
using Microsoft.Practices.Unity;

namespace AssetSite.admin.AdditionalScreens
{
    public partial class AdditionalScreenStep4 : ScreensController
    {
        [Dependency]
        public IPanelsService PanelsService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(),
                "_DlgInitalize_" + this.ClientID, "$(function () {$('#" + this.DialogContainer.ClientID + "').dialog({ autoOpen: false, width: 520, buttons: { 'Cancel' : function() { $(this).dialog(\'close\'); }, 'Ok': function(){ SavePanel(" 
                + _currentScreen.DynEntityConfigUid + "," + _currentScreen.ScreenId + ",'" + DialogContainer.ClientID + "'); } } });});",
                true);
            lbtnNew.Attributes.Add("onclick", "return ShowDialog(0,'" + DialogContainer.ClientID + "');");

            BindPanels();
        }

        protected void OnDeleteCommand(object sender, CommandEventArgs e)
        {
            long panelId = Convert.ToInt64(e.CommandArgument);
            PanelsService.Delete(panelId);
            this.BindPanels();
        }

        protected void lbRebind_Click(object sender, EventArgs e)
        {
            this.BindPanels();
        }

        private void BindPanels()
        {
            var sortPanels = PanelsService.GetAllByScreenId(_currentScreen.ScreenId)
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.UID)
                .ToList();
            byte index = 0;
            foreach (var item in sortPanels)
            {
                item.DisplayOrder = index;
                PanelsService.Save(item.Base);
                index++;
            }
            gvPanels.DataSource = sortPanels;
            gvPanels.DataBind();
        }

        public string GetEditScript(object UID)
        {
            return "return ShowDialog(" + UID.ToString() + ",'" + DialogContainer.ClientID + "');";
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            ScreensService.Save(_currentScreen);
            Response.Redirect(string.Format("AdditionalScreenStep5.aspx?ScreenId={0}&atuid={1}",
                _currentScreen.ScreenId, _currentType.UID));
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("AdditionalScreenStep3.aspx?ScreenId={0}&atuid={1}",
                _currentScreen.ScreenId, _currentType.UID));
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            if (!TryRedirectToBatch())
                Response.Redirect("~/Wizard/EditAssetType.aspx");
        }

        public void gvPanelsRowBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var panel = e.Row.DataItem as AppFramework.Core.Classes.Panel;
                var translation = new TranslatableString(panel.Name).GetTranslation();   // TODO: move to enity level via partial class?
                (e.Row.Cells[0].FindControl("TranslatedName") as Literal).Text = translation;
            }
        }

        protected void gvPanels_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            long uid = Convert.ToInt64(e.CommandArgument);
            var sortPanels = PanelsService
                .GetAllByScreenId(_currentScreen.ScreenId)
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.UID)
                .ToList();
            var item = sortPanels.FirstOrDefault(a => a.UID == uid);
            int index = sortPanels.IndexOf(item);
            if (e.CommandName == "down" && index + 1 != sortPanels.Count)
            {
                sortPanels[index].DisplayOrder = (byte)(index + 1);
                sortPanels[index + 1].DisplayOrder = (byte)index;
                PanelsService.Save(sortPanels[index].Base);
                PanelsService.Save(sortPanels[index + 1].Base);
            }
            else if (e.CommandName == "up" && index != 0)
            {
                sortPanels[index].DisplayOrder = (byte)(index - 1);
                sortPanels[index - 1].DisplayOrder = (byte)index;
                PanelsService.Save(sortPanels[index].Base);
                PanelsService.Save(sortPanels[index - 1].Base);
            }
            BindPanels();
        }
    }
}