using System;

namespace AssetManager.Infrastructure
{
    public interface IEnvironmentSettings
    {
        string PageNumber { get; set; }

        string PageSize { get; set; }

        string GetSiteRoot();

        string GetPathToAssetPage(long assetTypeId, long assetId);

        string Escape(string stringToEscape);

        string GetDocsUploadDirectory(long assetTypeId, long attributeId);

        string GetImagesUploadDirectory(long assetTypeId, long attributeId);

        string GetCacheDirectory();
    }
}
