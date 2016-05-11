using System.Collections.Generic;
using System.Data.SqlClient;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.DTO;
using System;

namespace AppFramework.Core.Classes
{
    public interface IAssetsService
    {
        Asset InsertAsset(Asset asset);
        Asset InsertAsset(Asset asset, IDictionary<AssetAttribute, Asset> dependencies);

        void DeleteAsset(Asset asset);
        void RestoreAsset(Asset asset);

        Asset CreateAsset(AssetType assetType);
        Asset CreateAsset(PredefinedEntity predefined);
        Asset GetAssetById(long assetId, AssetType assetType);
        Asset GetPredefinedAssetById(long assetId, PredefinedEntity predefined);
        Asset GetAssetByIdAndRevison(long assetId, AssetType assetType, int revision);
        Asset GetAssetByUid(long assetUid, AssetType assetType);
        Asset GetAssetByParameters(AssetType assetType, List<SqlParameter> parameters);
        Asset GetAssetByParameters(AssetType assetType, Dictionary<string, string> parameters);
        Asset GetFirstActiveAsset(AssetType assetType);

        IEnumerable<Asset> GetHistoryAssets(long assetTypeId, long assetId);

        IEnumerable<Asset> GetAssetsByAssetType(AssetType assetType, int? rowStart = null,
            int? rowsNumber = null);

        IEnumerable<Asset> GetAssetsByAssetTypeIdAndUser(long assetTypeId, long currentUserId, Func<Asset, bool> filterPredicate = null);
        IEnumerable<Asset> GetAssetsByAssetTypeAndUser(AssetType assetType, long currentUserId, Func<Asset, bool> filterPredicate = null);
        IEnumerable<Asset> GetAssetsByAssetTypeIdAndLocation(long assetTypeId, long locationId);
        IEnumerable<Asset> GetAssetsByParameters(AssetType assetType, Dictionary<string, string> parameters);
        IEnumerable<KeyValuePair<long, string>> GetIdNameListByAssetType(AssetType assetType);
        Asset GetRelatedAssetByAttribute(AssetAttribute attribute);
        IEnumerable<PlainAssetDTO> GetRelatedAssetsByAttribute(AssetAttribute attribute);
        bool TryGetAssetById(long assetId, AssetType assetType, out Asset asset);
    }
}