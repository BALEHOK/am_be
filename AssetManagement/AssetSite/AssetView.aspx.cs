using AppFramework.Core.Classes;
using System;
using Microsoft.Practices.Unity;
using AppFramework.Reports;

namespace AssetSite
{
    public partial class AssetView : BasePage
    {
        [Dependency]
        public IDynamicListsService DynamicListsService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            assetTreeView.AssetTypeSelectedEvent += assetTreeView_AssetTypeSelectedEvent;
            if (!String.IsNullOrEmpty(Request.QueryString["AstType"]))
            {
                long id = 0;
                if (long.TryParse(Request.QueryString["AstType"], out id))
                {
                    assetTreeView.SelectedAssetTypeId = id;
                }
            }

            if (assetTreeView.SelectedAssetTypeId > 0)
            {
                var assetType = AssetTypeRepository.GetById(assetTreeView.SelectedAssetTypeId);
                var asset = AssetsService.GetFirstActiveAsset(assetType);
                if (asset != null)
                {
                    assetsGrid.CreateColumns(asset);
                    assetsGrid.Visible = true;
                    lblAssetTypeName.Text = asset.GetConfiguration().Name;
                }
                hplCreateAsset.Visible = AuthenticationService.IsWritingAllowed(assetType);
            }
            else
            {
                assetsGrid.Visible = false;
            }

            string requestParams = Request.Params.Get("__EVENTARGUMENT");
            if (requestParams != null && requestParams != string.Empty)
            {
                HandleCustomRequest(requestParams);
            }
        }

        void assetTreeView_AssetTypeSelectedEvent(AssetType _assetType, TaxonomyItem item)
        {
            assetsGrid.PageIndex = 0;
            lblAssetTypeName.Text = _assetType.Name;
            lblCreateAsset.Text = "Add " + _assetType.Name;
            hplCreateAsset.NavigateUrl = "/Asset/New/Step2.aspx?atid=" + _assetType.ID;
            lnkRenderReport.Visible = true;
            lnkRenderReport.NavigateUrl = 
                string.Format("~/Reports/Render.aspx?AssetTypeId={0}&ReportType={1}", 
                    _assetType.ID, (int)ReportType.AssetsListReport);
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
                        long id = assetTreeView.SelectedAssetTypeId;
                        if (long.TryParse(keyValue[1], out assetId) && assetId > 0 && id > 0)
                        {
                            var at = AssetTypeRepository.GetById(id);
                            var asset = AssetsService.GetAssetById(assetId, at);
                            var permission = AuthenticationService.GetPermission(asset);
                            AssetsService.DeleteAsset(asset, permission);
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
