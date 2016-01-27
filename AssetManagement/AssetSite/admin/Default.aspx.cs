using System;
using System.Web.Security;
using System.Web.UI;
using AppFramework.Core.Classes;
using AssetSite.Helpers;
using AssetSite.MasterPages;

namespace AssetSite.admin
{
    public partial class Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            linkLocationMove.Visible = menuMobile.Visible =
                ApplicationSettings.ApplicationType != AppFramework.ConstantsEnumerators.ApplicationType.SOBenBUB;

            if (Roles.IsUserInRole("Super User"))
            {
                panelConfigureObjects.Visible = false;
                panelConfigureTaxonomies.Visible = false;
                linkTypesAndValidation.Visible = false;
                linkSynchronization.Visible = false;
                linkSearch.Visible = false;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            ((MasterPageBase)Page.Master.Master).BodyClass = "adminmain";
            ((MasterPageBase)Page.Master.Master).DisplayRightColumn = false;
            ((MasterPageBase)Page.Master.Master).DisplayLeftColumn = false;
            base.OnPreRender(e);
        }

        protected void NewAT_Click(object sender, EventArgs e)
        {
            SessionWrapper.CleanWizardSession();
            Response.Redirect("~/Wizard/Step1.aspx");
        }
    }
}