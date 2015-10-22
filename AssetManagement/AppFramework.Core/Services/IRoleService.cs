using AppFramework.ConstantsEnumerators;
using System.Collections.Generic;

namespace AppFramework.Core.Services
{
    public interface IRoleService
    {
        IEnumerable<PredefinedRoles> GetAllRoles();
    }
}