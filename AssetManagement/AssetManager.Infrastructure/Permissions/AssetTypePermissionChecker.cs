using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Exceptions;
using AppFramework.DataProxy;
using AppFramework.Entities;

namespace AssetManager.Infrastructure.Permissions
{
    public class AssetTypePermissionChecker : IAssetTypePermissionChecker
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssetTypePermissionChecker(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Permission GetPermission(long assetTypeId, long userId)
        {
            var temp = new[] {assetTypeId};
            var permission = Permission.DDDD;
            if (FilterWritePermitted(temp, userId).Any())
            {
                permission = permission | Permission.RDDD;
            }

            if (FilterWritePermitted(temp, userId).Any())
            {
                permission = permission | Permission.DWDD;
            }

            return permission;
        }

        public void EnsureReadPermission(DynEntityConfig assetType, long userId)
        {
            if (FilterReadPermitted(new[] { assetType }, userId).ToArray().Length == 0)
            {
                throw new InsufficientPermissionsException("No read permissions on the asset type");
            }
        }

        public void EnsureWritePermission(DynEntityConfig assetType, long userId)
        {
            if (FilterWritePermitted(new[] {assetType}, userId).ToArray().Length == 0)
            {
                throw new InsufficientPermissionsException("No write permissions on the asset type");
            }
        }

        public IEnumerable<DynEntityConfig> FilterReadPermitted(IEnumerable<DynEntityConfig> assetTypes, long userId)
        {
            return FilterPermitted(assetTypes, userId, Permission.RDDD);
        }

        public IEnumerable<long> FilterReadPermitted(IEnumerable<long> assetTypeIds, long userId)
        {
            return FilterPermitted(assetTypeIds, userId, Permission.RDDD);
        }

        public IEnumerable<DynEntityConfig> FilterWritePermitted(IEnumerable<DynEntityConfig> assetTypes, long userId)
        {
            return FilterPermitted(assetTypes, userId, Permission.DWDD);
        }

        public IEnumerable<long> FilterWritePermitted(IEnumerable<long> assetTypeIds, long userId)
        {
            return FilterPermitted(assetTypeIds, userId, Permission.DWDD);
        }

        private IEnumerable<DynEntityConfig> FilterPermitted(IEnumerable<DynEntityConfig> assetTypes, long userId,
            Permission permission)
        {
            var permittedAssetTypes = _unitOfWork.GetPermittedAssetTypes(userId, (byte) permission).ToList();
            return assetTypes.Where(t => permittedAssetTypes.Any(id => id == t.DynEntityConfigId));
        }

        private IEnumerable<long> FilterPermitted(IEnumerable<long> assetTypeIds, long userId,
            Permission permission)
        {
            var permittedAssetTypes = _unitOfWork.GetPermittedAssetTypes(userId, (byte) permission).ToList();
            return assetTypeIds.Where(id => permittedAssetTypes.Any(permittedId => id == permittedId));
        }
    }
}