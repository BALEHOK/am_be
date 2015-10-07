using System;
using System.Web.UI.WebControls;
using AppFramework.Core.AC.Authentication;

namespace AppFramework.Core.Classes.Reports
{
    public class DropDownFilterControl : DropDownList, IFilterControl
    {
        public DropDownFilterControl(
            ReportField field,
            IAuthenticationService authenticationService,
            IAssetTypeRepository assetTypeRepository,
            IAssetsService assetsService)
        {
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            if (authenticationService == null)
                throw new ArgumentNullException("authenticationService");
            
            AssetTypeAttribute attr = field.GetTypeAttribute();
            if (attr != null && attr.IsAsset && attr.RelatedAssetTypeID != null)
            {
                var assetType = assetTypeRepository.GetById((long) attr.RelatedAssetTypeID);
                var assets = assetsService.GetAssetsByAssetTypeAndUser(assetType, authenticationService.CurrentUserId);
                DataSource = assets;
                DataBind();
            }
        }

        public override string DataTextField
        {
            get
            {
                return "Name";
            }
        }

        public override string DataValueField
        {
            get
            {
                return "UID";
            }
        }

        #region IFilterControl Members

        public string GetValue()
        {
            return this.SelectedValue;
        }

        public string GetText()
        {
            return this.SelectedItem.Text;
        }

        #endregion
    }
}
