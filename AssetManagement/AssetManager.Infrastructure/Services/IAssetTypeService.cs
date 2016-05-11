using System.Collections.Generic;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models.TypeModels;

namespace AssetManager.Infrastructure.Services
{
    public interface IAssetTypeService
    {
        IEnumerable<AssetTypeModel> GetAssetTypes(long userId, bool loadAttributes = false, bool loadScreens = false);
        AssetTypeModel GetAssetType(long userId, long id, bool loadAttributes = false);
        IEnumerable<DynEntityConfig> GetAssetTypesByIds(long userId, IEnumerable<long> typesToload);
        IEnumerable<AssetTypeModel> GetWritableAssetTypes(long userId, bool loadAttributes = false, bool loadScreens = false);
        IEnumerable<DynEntityAttribConfig> GetChildAttribs(long userId, long configId);
        IEnumerable<AssetTypeModel> GetReservableAssetTypes(long userId);
    }
}