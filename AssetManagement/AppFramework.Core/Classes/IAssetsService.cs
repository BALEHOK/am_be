using System.Collections.Generic;
using System.Data.SqlClient;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.DTO;
using AppFramework.ConstantsEnumerators;
using AppFramework.Entities;

namespace AppFramework.Core.Classes
{
    public interface IAssetsService
    {
        Asset InsertAsset(Asset asset);
        Asset InsertAsset(Asset asset, IDictionary<AssetAttribute, Asset> dependencies);

        void DeleteAsset(Asset deletingAsset, Permission permission);
        void RestoreAsset(Asset asset);

        Asset CreateAsset(AssetType assetType);
        Asset CreateAsset(PredefinedEntity predefined);
        Asset GetAssetById(long assetId, AssetType assetType);
        Asset GetAssetByIdAndRevison(long assetId, AssetType assetType, int revision);
        Asset GetAssetByUid(long assetUid, AssetType assetType);
        Asset GetAssetByUid(long assetUid, long assetTypeUid);
        Asset GetAssetByParameters(AssetType assetType, List<SqlParameter> parameters);
        Asset GetAssetByParameters(AssetType assetType, Dictionary<string, string> parameters);
        Asset GetFirstActiveAsset(AssetType assetType);
        IEnumerable<Asset> GetHistoryAssets(long assetTypeId, long assetId);        
        IEnumerable<Asset> GetAssetsByAssetType(AssetType assetType, int? rowStart = null,
            int? rowsNumber = null);
        IEnumerable<Asset> GetAssetsByAssetTypeIdAndUser(long assetTypeUid, long currentUserId);
        IEnumerable<Asset> GetAssetsByAssetTypeAndUser(AssetType assetType, long currentUserId);
        IEnumerable<Asset> GetAssetsByAssetTypeIdAndLocation(long assetTypeId, long locationId);
        IEnumerable<Asset> GetAssetsByParameters(AssetType assetType, Dictionary<string, string> parameters);
        IEnumerable<KeyValuePair<long, string>> GetIdNameListByAssetType(AssetType assetType);
        Asset GetRelatedAssetByAttribute(AssetAttribute attribute);
        IEnumerable<PlainAssetDTO> GetRelatedAssetsByAttribute(AssetAttribute attribute);
    }
}