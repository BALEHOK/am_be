using AppFramework.ConstantsEnumerators;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace AssetSite.Documents
{
    public partial class Default : BasePage
    {
        private long _assetTypeId;

        protected void Page_Load(object sender, EventArgs e)
        {
            var assetType = AssetTypeRepository.GetPredefinedAssetType(PredefinedEntity.Document);
            _assetTypeId = assetType.ID;
            var asset = AssetsService.GetAssetsByAssetTypeAndUser(assetType, AuthenticationService.CurrentUserId).FirstOrDefault();
            if (asset != null)
            {
                assetsGrid.CreateColumns(asset);
                assetsGrid.Visible = true;
                if (!IsPostBack)
                    assetsDataSource.SelectParameters.Add(new Parameter("assetTypeId", System.Data.DbType.Int64, _assetTypeId.ToString()));
            }

            string requestParams = Request.Params.Get("__EVENTARGUMENT");
            if (requestParams != null && requestParams != string.Empty)
            {
                HandleCustomRequest(requestParams);
            }
        }

        protected void DataSource_Selecting(Object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (!e.InputParameters.Contains("assetTypeId"))
            {
                e.InputParameters.Add("assetTypeId", _assetTypeId);
            }
        }

        private void HandleCustomRequest(string requestParams)
        {
            string[] keyValue = requestParams.Split('$');
            if (keyValue.Length == 2)
            {
                switch (keyValue[0])
                {
                    case "delete":
                        long assetId = 0;
                        if (long.TryParse(keyValue[1], out assetId) && assetId > 0 && _assetTypeId > 0)
                        {
                            var at = AssetTypeRepository.GetById(_assetTypeId);
                            var asset = AssetsService.GetAssetById(assetId, at);
                            var permission = AuthenticationService.GetPermission(asset);
                            AssetsService.DeleteAsset(asset);
                        }
                        break;

                    default:
                        break;
                }
            }

        }
    }
}