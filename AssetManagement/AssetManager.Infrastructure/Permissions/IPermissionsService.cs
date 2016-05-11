using System.Collections.Generic;
using AppFramework.Core.Classes;

namespace AssetManager.Infrastructure.Permissions
{
    public interface IPermissionsService
    {
        List<RightsEntry> GetUserRights(long userId);
    }
}