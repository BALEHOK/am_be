/*--------------------------------------------------------
* InMemoryAuthenticationStorageProvider.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 7/30/2009 1:05:06 PM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

namespace AppFramework.Core.AC.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using AppFramework.Core.AC.Authentication;

    public class InMemoryAuthenticationStorageProvider : IAuthenticationStorageProvider
    {
        // private AuthenticationStorage _data;
        private bool _userRightsIsActual;

        public InMemoryAuthenticationStorageProvider()
        {
            if (InMemoryStorageContainer.Instance.Data == null)
                InMemoryStorageContainer.Instance.Data = new AuthenticationStorage();
            this.IsActual = false;
        }

        #region IAuthenticationStorageProvider Members

        public AppFramework.Core.AC.Authentication.AuthenticationStorage GetStorage()
        {
            return InMemoryStorageContainer.Instance.Data as AuthenticationStorage;
        }

        public void SaveStorage(AppFramework.Core.AC.Authentication.AuthenticationStorage storage)
        {
            InMemoryStorageContainer.Instance.Data = storage;
        }

        public bool IsActual
        {
            get
            {
                return this._userRightsIsActual;
            }
            set
            {
                this._userRightsIsActual = value;
            }
        }

        #endregion
    }
}
