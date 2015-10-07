using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Stock;
using System;
using System.Linq;
using System.Web.UI;

namespace AssetSite.Asset
{
    public class LocationInfo
    {
        public string LocationName { get; set; }
        public long Rest { get; set; }
        public string LocationUrl { get; set; }
    }

    public partial class ViewTransactions : BasePage
    {
        public AssetType assetType;
        public AppFramework.Core.Classes.Asset asset;

        protected void Page_Load(object sender, EventArgs e)
        {
            long assetTypeUID = 0;
            long assetUID = 0;
            long assetTypeID = 0;
            long assetID = 0;

            // get by UIDs
            if (Request.QueryString["assetTypeUID"] != null
                && Request.QueryString["assetUID"] != null)
            {
                long.TryParse(Request.QueryString["assetTypeUID"].ToString(), out assetTypeUID);
                long.TryParse(Request.QueryString["assetUID"].ToString(), out assetUID);
                assetType = AssetTypeRepository.GetByUid(assetTypeUID);
                asset = AssetsService.GetAssetByUid(assetUID, assetType);
            }
            // get by IDs
            else if (Request.QueryString["assetTypeID"] != null
                && Request.QueryString["assetID"] != null)
            {
                long.TryParse(Request.QueryString["assetTypeID"].ToString(), out assetTypeID);
                long.TryParse(Request.QueryString["assetID"].ToString(), out assetID);

                assetType = AssetTypeRepository.GetById(assetTypeID);
                asset = AssetsService.GetAssetById(assetID, assetType);
            }
            else
            {
                Response.Redirect("~/AssetView.aspx");
            } 

            if (!IsPostBack)
            {
                UpdateByLocationList(asset.ID, assetType.ID);

                TransactionsList.DataSource = StockTransaction.GetAll(assetType.ID, asset.ID).OrderByDescending(t => t.TransactionDate);
                TransactionsList.DataBind();

                var report = new StockSubtotalReport(assetType.ID, asset.ID);
                SubtotalGrid.DataSource = report.GetAll();
                SubtotalGrid.DataBind();
            }
        }

        private void UpdateByLocationList(long assetId, long configId)
        {
            var at = AssetTypeRepository.GetPredefinedAssetType(PredefinedEntity.Location);
            var locationsList = AssetsService.GetAssetsByAssetTypeAndUser(at, AuthenticationService.CurrentUserId).ToList();
            var manager = new StockTransactionManager();
            var available = manager.GetAvailableLocationsFor(assetId, configId);
            locationsList = locationsList.Where(location => available.ContainsKey(location.ID)).ToList();

            var items = locationsList.Select(l => new LocationInfo() { LocationName = l.Name, LocationUrl = Page.ResolveUrl(l.NavigateUrl), Rest = (long)available[l.ID] });
            this.SubtotalGridByLocation.DataSource = items;
            this.SubtotalGridByLocation.DataBind();
        }
    }
}
