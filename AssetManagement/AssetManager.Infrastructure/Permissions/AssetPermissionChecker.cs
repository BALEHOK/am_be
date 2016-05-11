using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.DataProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.Core.Exceptions;

namespace AssetManager.Infrastructure.Permissions
{
    public class AssetPermissionChecker : IAssetPermissionChecker
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IPermissionsService _permissionsService;
        private readonly IAssetsService _assetsService;

        public AssetPermissionChecker(
            IUnitOfWork unitOfWork,
            IPermissionsService permissionsService,
            IAssetTypeRepository assetTypeRepository,
            IAssetsService assetsService)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (permissionsService == null)
                throw new ArgumentNullException("permissionsService");
            _permissionsService = permissionsService;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
        }

        public void EnsureReadPermission(Asset asset, long userId)
        {
            if (!HasPermission(asset, userId, Permission.RDDD))
            {
                throw new InsufficientPermissionsException("No read permissions on the asset");
            }
        }

        public void EnsureWritePermission(Asset asset, long userId, bool isFinancial = false)
        {
            var permission = isFinancial ? Permission.DDDW : Permission.DWDD;

            if (!HasPermission(asset, userId, permission))
            {
                throw new InsufficientPermissionsException("No write permissions on the asset");
            }
        }

        public void EnsureDeletePermission(Asset asset, long userId)
        {
            if (!HasPermission(asset, userId, Permission.ReadWriteDelete))
            {
                throw new InsufficientPermissionsException("No delete permissions on the asset");
            }
        }

        private bool HasPermission(Asset asset, long userId, Permission permission)
        {
            var actualPermission = GetPermission(asset, userId);
            return actualPermission.CompareWithMask(permission.GetCode());
        }

        public Permission GetPermission(Asset asset, long userId)
        {
            if (asset == null)
                throw new ArgumentNullException("asset");

            var userType = _assetTypeRepository.GetPredefinedAssetType(PredefinedEntity.User);
            var currentUser = _assetsService.GetAssetById(userId, userType);
            var permissionsOnUsers = PermissionsProvider.GetByCode(
                Convert.ToByte(currentUser["PermissionOnUsers"].Value));

            long? deptId = null;
            if (asset[AttributeNames.DepartmentId] != null && asset[AttributeNames.DepartmentId].ValueAsId.HasValue)
                deptId = asset[AttributeNames.DepartmentId].ValueAsId;

            var taxonomyItemsIds = _unitOfWork
                .TaxonomyItemRepository
                .GetTaxonomyItemsByAssetTypeId(asset.GetConfiguration().ID)
                .Select(x => x.TaxonomyItemId)
                .ToList();

            var isUser = _assetTypeRepository.IsPredefinedAssetType(
                asset.Configuration, PredefinedEntity.User);
            Permission result;
            var employeeIds = _unitOfWork.GetUsersTree(userId);

            // if provided asset is user (and child employee), return the rights for him;            
            if (isUser && (employeeIds.Any(u => u == asset.ID) || asset.ID == 0))
            {
                result = permissionsOnUsers;
            }
            // if provided asset belongs to current user or to his child employee            
            else if (IsBelonging(asset, userId, employeeIds))
            {
                var rights = _permissionsService.GetUserRights(userId);
                result = PermissionsProvider.Or(
                    permissionsOnUsers,
                    rights.GetPermission(asset.Configuration.ID, deptId, taxonomyItemsIds));
            }
            else
            {
                // in other cases look for permission in Rights list     
                var rights = _permissionsService.GetUserRights(userId);
                result = rights.GetPermission(asset.Configuration.ID, deptId, taxonomyItemsIds);
            }
            return result;
        }

        private static bool IsBelonging(Asset asset, long userId, List<long> employeeIds)
        {
            long? ownerId = null;
            if (asset[AttributeNames.OwnerId] != null && asset[AttributeNames.OwnerId].ValueAsId.HasValue)
                ownerId = asset[AttributeNames.OwnerId].ValueAsId;

            long? assetUserId = null;
            if (asset[AttributeNames.UserId] != null && asset[AttributeNames.UserId].ValueAsId.HasValue)
                assetUserId = asset[AttributeNames.UserId].ValueAsId;

            return (ownerId.HasValue && employeeIds.Any(x => x == ownerId.Value)) ||
                (assetUserId.HasValue && (employeeIds.Any(x => x == assetUserId.Value)) ||
                asset.ID == ownerId ||
                asset.ID == assetUserId) ||
                asset[AttributeNames.UpdateUserId].ValueAsId == userId;
        }
    }
}
