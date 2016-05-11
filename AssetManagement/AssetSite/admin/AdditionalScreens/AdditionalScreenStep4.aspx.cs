using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes.Extensions;
using AppFramework.Core.Services;
using AppFramework.Entities;
using AssetManager.Infrastructure.Services;
using Microsoft.Practices.Unity;

namespace AssetSite.admin.AdditionalScreens
{
    public partial class AdditionalScreenStep4 : ScreensController
    {
        [Dependency]
        public IPanelsService PanelsService { get; set; }

        [Dependency]
        public IAssetTypeService AssetTypeService { get; set; }

        protected List<DynEntityAttribConfig> ChildAttributes;

        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            ChildAttributes = AssetTypeService.GetChildAttribs(AuthenticationService.CurrentUserId, _currentType.ID)
                .ToList();

            Page.ClientScript.RegisterClientScriptBlock(GetType(),
                "_DlgInitalize_" + ClientID,
                "$(function () {$('#" + DialogContainer.ClientID +
                "').dialog({ autoOpen: false, width: 520, buttons: { 'Cancel' : function() { $(this).dialog(\'close\'); }, 'Ok': function(){ SavePanel("
                + _currentScreen.DynEntityConfigUid + "," + _currentScreen.ScreenId + ",'" + DialogContainer.ClientID +
                "'); } } });});",
                true);
            lbtnNew.Attributes.Add("onclick", "return ShowDialog(0,'" + DialogContainer.ClientID + "');");

            BindPanels(false);
        }

        protected void OnDeleteCommand(object sender, CommandEventArgs e)
        {
            var panelId = Convert.ToInt64(e.CommandArgument);
            PanelsService.Delete(panelId);
            BindPanels(true);
        }

        private void BindPanels(bool reindex)
        {
            var sortPanels = PanelsService.GetAllByScreenId(_currentScreen.ScreenId);

            if (reindex)
            {
                byte index = 0;
                foreach (var item in sortPanels)
                {
                    item.DisplayOrder = index;
                    PanelsService.Save(item);
                    index++;
                }
            }

            gvPanels.DataSource = sortPanels;
            gvPanels.DataBind();
        }

        public string GetEditScript(object UID)
        {
            return "return ShowDialog(" + UID + ",'" + DialogContainer.ClientID + "');";
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

        protected void gvPanelsRowBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var panel = (AttributePanel) e.Row.DataItem;
                ((Literal) e.Row.Cells[0].FindControl("TranslatedName")).Text = panel.Name.Localized();
            }
        }

        protected void gvPanels_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var uid = Convert.ToInt64(e.CommandArgument);
            var sortPanels = PanelsService.GetAllByScreenId(_currentScreen.ScreenId);
            var item = sortPanels.FirstOrDefault(a => a.AttributePanelUid == uid);
            var index = sortPanels.IndexOf(item);
            if (e.CommandName == "down" && index + 1 != sortPanels.Count)
            {
                sortPanels[index].DisplayOrder = (byte) (index + 1);
                sortPanels[index + 1].DisplayOrder = (byte) index;
                PanelsService.Save(sortPanels[index]);
                PanelsService.Save(sortPanels[index + 1]);
            }
            else if (e.CommandName == "up" && index != 0)
            {
                sortPanels[index].DisplayOrder = (byte) (index - 1);
                sortPanels[index - 1].DisplayOrder = (byte) index;
                PanelsService.Save(sortPanels[index]);
                PanelsService.Save(sortPanels[index - 1]);
            }

            BindPanels(true);
        }
    }
}