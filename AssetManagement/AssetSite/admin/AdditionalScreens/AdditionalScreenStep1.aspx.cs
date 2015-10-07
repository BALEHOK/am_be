using System;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.ScreensServices;
using Microsoft.Practices.Unity;

namespace AssetSite.admin.AdditionalScreens
{
    public partial class AdditionalScreenStep1 : ScreensController
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (IsPostBack)
            {
                gvScreens.DataBind();
            }

            AddNewScreenLink.NavigateUrl = 
                string.Format("AdditionalScreenStep2.aspx?atuid={0}",
                Request.QueryString["atuid"]);
        }

        public string GetEditUrl(object screenId)
        {
            return string.Format("AdditionalScreenStep2.aspx?ScreenId={0}&atuid={1}", 
                screenId, Request.QueryString["atuid"]);
        }

        public void gvScreens_DataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var screen = (AppFramework.Entities.AssetTypeScreen)e.Row.DataItem;
                // TODO: move to enity level via partial class?
                var translation = new TranslatableString(screen.Name).GetTranslation();
                var literal = (Literal)e.Row.Cells[0].FindControl("TranslatedName");
                literal.Text = translation;

                var deleteButton = (LinkButton)e.Row.FindControl("lbtnDelete");
                string postbackEvent = Page.ClientScript.GetPostBackEventReference(deleteButton, "");
                deleteButton.OnClientClick = "return ShowConfirmationDialog(function(){ " + postbackEvent + " });";
            }
        }

        protected void lbtnDelete_OnClick(object sender, EventArgs e)
        {
            ScreensService.Delete(long.Parse(((LinkButton)sender).CommandArgument.ToString()));
            gvScreens.DataBind();
        }
    }
}