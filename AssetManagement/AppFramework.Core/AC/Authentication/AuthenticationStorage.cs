using System.Collections.Generic;
using AppFramework.Core.Classes;

namespace AppFramework.Core.AC.Authentication
{
    /// <summary>
    /// Represents the storage for user rights data
    /// </summary>
    public class AuthenticationStorage
    {
        public List<AssetUser> Users { get; set; }

        public AuthenticationStorage()
        {
            Users = new List<AssetUser>(1);
        }
    }
}
