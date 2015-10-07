using AppFramework.Core.Classes;
using System.Collections.Generic;

namespace AppFramework.Core.Services
{
    public interface IRightsService
    {
        void SetPermissionsForUser(IEnumerable<RightsEntry> list, long userId, long currentUserId, long viewId = 0);
    }
}