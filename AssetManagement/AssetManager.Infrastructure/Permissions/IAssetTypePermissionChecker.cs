using System.Collections.Generic;
using AppFramework.Core.AC.Authentication;
using AppFramework.Entities;

namespace AssetManager.Infrastructure.Permissions
{
    public interface IAssetTypePermissionChecker
    {
        IEnumerable<DynEntityConfig> FilterWritePermitted(IEnumerable<DynEntityConfig> assetTypes, long userId);
        IEnumerable<DynEntityConfig> FilterReadPermitted(IEnumerable<DynEntityConfig> assetTypes, long userId);
        Permission GetPermission(long assetTypeId, long userId);
        void EnsureReadPermission(DynEntityConfig assetType, long userId);
        void EnsureWritePermission(DynEntityConfig assetType, long userId);
        IEnumerable<long> FilterReadPermitted(IEnumerable<long> assetTypeIds, long userId);
        IEnumerable<long> FilterWritePermitted(IEnumerable<long> assetTypeIds, long userId);
    }
}