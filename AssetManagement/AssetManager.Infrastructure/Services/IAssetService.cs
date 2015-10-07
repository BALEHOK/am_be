using AssetManager.Infrastructure.Models;
using System;
using System.Collections.Generic;

namespace AssetManager.Infrastructure.Services
{
    public interface IAssetService
    {
        AssetModel GetAsset(long assetTypeId, long assetId, int? revision = null, long? uid = null);

        AttributeModel GetAssetAttribute(long assetTypeId, long assetId, long attributeId);
        
        IEnumerable<AssetAttributeRelatedEntitiesModel> GetAssetRelatedEntities(
            long assetTypeId, long? assetId, int? revision = null, long? uid = null);

        AssetHistoryModel GetAssetHistory(long assetTypeId, long assetId);

        Tuple<long, string> SaveAsset(AssetModel model, long userId, long? screenId = null);

        void DeleteAsset(long assetTypeId, long assetId);

        void RestoreAsset(long assetTypeId, long assetId);

        IEnumerable<AssetModel> GetAssets(long assetTypeId, long userId, string query, int? rowStart, int? rowsNumber);

        AssetModel CreateAsset(long assetTypeId, long userId);

        AssetModel CalculateAsset(AssetModel model, long userId, long? screenId = null, bool overwrite = false);
    }
}
