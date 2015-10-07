using System.Collections.Generic;
using AppFramework.Core.Classes;

namespace AppFramework.Core.AC.Authentication
{
    public interface IAccessManager
    {
        /// <summary>
        /// AuthenticationService
        /// </summary>
        IAuthenticationService AuthenticationService { get; }
     
        /// <summary>
        /// Check if the rights storage is up-to-date
        /// </summary>
        bool IsActual { get; set; }


        /// <summary>
        /// Clear all personal data for user
        /// </summary>
        void ClearPersonalData();
        
        /// <summary>
        /// Sets the rights for provided user 
        /// to the provided rights storage
        /// </summary>
        /// <param name="rights"></param>
        /// <param name="userID"></param>
        void InitRights();

        /// <summary>
        /// Forces user to update his rights.
        /// </summary>
        /// <param name="userId"></param>
        void ForceRightsUpdate(long userId);

        /// <summary>
        /// Forces logout of user with provided ID.
        /// </summary>
        /// <param name="userId"></param>
        void ForceLogout(long userId);
    }
}