using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.Core.AC.Authentication;

namespace AppFramework.Core.AC.Providers
{
    public interface IAuthenticationStorageProvider
    {
        AuthenticationStorage GetStorage();
        void SaveStorage(AuthenticationStorage storage);
        bool IsActual { get; set; }
    }
}
