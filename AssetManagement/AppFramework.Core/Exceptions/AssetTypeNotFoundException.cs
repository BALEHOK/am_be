namespace AppFramework.Core.Exceptions
{
    public class AssetTypeNotFoundException : EntityNotFoundException
    {
        /// <summary>
        /// Asset type not found
        /// </summary>
        /// <param name="assetTypeId">id of requested asset type (if null, not included in message)</param>
        /// <param name="assetTypeUid">uid of requested asset type (if null, not included in message)</param>
        public AssetTypeNotFoundException(long? assetTypeId, long? assetTypeUid)
            : base(
                "Asset type " +
                (assetTypeId.HasValue ? assetTypeId + " " : string.Empty) +
                (assetTypeUid.HasValue ? "(" + assetTypeUid + ") " : string.Empty) +
                "not found in the system.")
        {
        }
    }
}