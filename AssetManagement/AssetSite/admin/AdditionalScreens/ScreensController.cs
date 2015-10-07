using AppFramework.Core.Classes;
using AppFramework.Core.Classes.ScreensServices;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Entities;
using Microsoft.Practices.Unity;
using System;

namespace AssetSite.admin.AdditionalScreens
{
    public class ScreensController : BasePage
    {
        [Dependency]
        public IScreensService ScreensService { get; set; }

        protected AssetTypeScreen _currentScreen;
        protected AssetType _currentType;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request["atuid"]))
                Response.Redirect("~/Wizard/EditAssetType.aspx");
            _currentType = AssetTypeRepository.GetByUid(long.Parse(Request["atuid"]));

            _currentScreen = new AssetTypeScreen();
            if (Request["ScreenId"] != null && long.Parse(Request["ScreenId"]) > 0)
                _currentScreen = ScreensService.GetScreenById(long.Parse(Request["ScreenId"]));
        }

        protected bool TryRedirectToBatch()
        {
            if (Session[SessionVariables.AssetTypeWizard_BatchUrl] != null)
            {
                var batchUrl = Session[SessionVariables.AssetTypeWizard_BatchUrl].ToString();
                Session[SessionVariables.AssetTypeWizard_BatchUrl] = null;
                Response.Redirect(batchUrl);
                return true;
            }
            return false;
        }
    }
}