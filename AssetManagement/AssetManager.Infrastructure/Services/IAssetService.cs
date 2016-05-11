using AppFramework.Core.Classes;
using AssetManager.Infrastructure.Models;
using System;
using System.Collections.Generic;

namespace AssetManager.Infrastructure.Services
{
    public interface IAssetService
    {
        AssetModel GetAsset(long assetTypeId, long assetId, long userId, int? revision = null, long? uid = null, bool withChildTypes = false);

        AttributeModel GetAssetAttribute(long assetTypeId, long assetId, long attributeId, long userId);
        
        IEnumerable<AssetAttributeRelatedEntitiesModel> GetAssetRelatedEntities(
            long assetTypeId, long? assetId, int? revision = null, long? uid = null);

        AssetHistoryModel GetAssetHistory(long assetTypeId, long assetId);

        Tuple<long, string> SaveAsset(AssetModel model, long userId, long? screenId = null);

        void DeleteAsset(long assetTypeId, long assetId, long userId);

        void RestoreAsset(long assetTypeId, long assetId);

        IEnumerable<AssetModel> GetAssets(long assetTypeId, long userId, Func<Asset, bool> filterPredicate = null,
            int? rowStart = null, int? rowsNumber = null);

        IEnumerable<AssetModel> GetAssetsByName(long assetTypeId, long userId, string query, 
            int? rowStart, int? rowsNumber);

        AssetModel CreateAsset(long assetTypeId, long userId);

        AssetModel CalculateAsset(AssetModel model, long userId, long? screenId = null);

        IEnumerable<ChildAssetType> GetRelatedAssetTypes(long userId, long assetTypeId);
        Asset UpdateAsset(Dictionary<long, EntityAttribConfigModel> assetModel, long userId);
    }
}
