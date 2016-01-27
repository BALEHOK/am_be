using System.Collections.Generic;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models.TypeModels;

namespace AssetManager.Infrastructure.Services
{
    public interface IAssetTypeService
    {
        TypesInfoModel GetAssetTypes(bool loadAttributes = false, bool loadScreens = false);
        AssetTypeModel GetAssetType(long id, bool loadAttributes = false);
        IEnumerable<DynEntityConfig> GetAssetTypesByIds(IEnumerable<long> typesToload);
    }
}