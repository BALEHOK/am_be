namespace AssetManager.Infrastructure
{
    public interface IEnvironmentSettings
    {
        string PageNumber { get; set; }

        string PageSize { get; set; }

        string GetSiteRoot();

        string GetPathToAssetPage(long assetTypeId, long assetId);

        string Escape(string stringToEscape);

        string GetDocsUploadBaseDir();

        string GetImagesUploadBaseDir();

        string GetCacheDirectory();

        string GetAssetMediaHttpRoot();

        string GetAssetMediaRelativePath(long assetTypeId, long attributeId);
        string GetBannerUploadBaseDir();
        string GetBannerRelativePath();
    }
}
