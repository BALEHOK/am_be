using AppFramework.Core.Classes;

namespace AssetSite.Reservations
{
    public class ReservationController : BasePage
    {
        protected AssetType AssetType;
        protected AppFramework.Core.Classes.Asset Asset;

        protected override void OnLoad(System.EventArgs e)
        {
            long assetTypeUID = 0;
            long assetUID = 0;
            long assetTypeID = 0;
            long assetID = 0;

            if (Request.QueryString["assetTypeUID"] != null
                && Request.QueryString["assetUID"] != null)
            {
                long.TryParse(Request.QueryString["assetTypeUID"].ToString(), out assetTypeUID);
                long.TryParse(Request.QueryString["assetUID"].ToString(), out assetUID);
                AssetType = AssetTypeRepository.GetByUid(assetTypeUID);
                Asset = AssetsService.GetAssetByUid(assetUID, AssetType);
            }
            else if (Request.QueryString["assetTypeID"] != null
               && Request.QueryString["assetID"] != null)
            {
                long.TryParse(Request.QueryString["assetTypeID"].ToString(), out assetTypeID);
                long.TryParse(Request.QueryString["assetID"].ToString(), out assetID);

                AssetType = AssetTypeRepository.GetById(assetTypeID);
                Asset = AssetsService.GetAssetById(assetID, AssetType);

                if (Asset == null)
                {
                    Response.Redirect("~/AssetView.aspx");
                }
            }
            else
            {
                Response.Redirect("~/AssetView.aspx");
            }
        }
    }
}