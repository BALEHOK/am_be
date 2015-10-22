using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.ConstantsEnumerators;

namespace AppFramework.Core.Services
{
    public class RoleService : IRoleService
    {
        public IEnumerable<PredefinedRoles> GetAllRoles()
        {
            return Enum.GetValues(typeof(PredefinedRoles)).Cast<PredefinedRoles>();
        }
    }
}
