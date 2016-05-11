using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;

namespace AssetManager.Infrastructure.Permissions
{
    public interface IAssetPermissionChecker
    {
        Permission GetPermission(Asset asset, long userId);

        void EnsureReadPermission(Asset asset, long userId);

        void EnsureDeletePermission(Asset asset, long userId);
        void EnsureWritePermission(Asset asset, long userId, bool isFinancial = false);
    }
}