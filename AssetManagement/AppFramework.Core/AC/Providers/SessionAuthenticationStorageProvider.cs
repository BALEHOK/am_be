using AppFramework.Core.AC.Authentication;
using System.Web;

namespace AppFramework.Core.AC.Providers
{
    /// <summary>
    /// Provides the interface for retrieving and saving 
    /// user rights storage to session
    /// </summary>
    public class SessionAuthenticationStorageProvider : IAuthenticationStorageProvider
    {
        public AuthenticationStorage GetStorage()
        {
            return HttpContext.Current.Session["UserRights"]
                as AuthenticationStorage ?? new AuthenticationStorage();          
        }

        public void SaveStorage(AuthenticationStorage storage)
        {
            HttpContext.Current.Session["UserRights"] = storage;
        }

        public bool IsActual
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["UserRightsIsActual"] != null)
                {
                    bool.TryParse(HttpContext.Current.Session["UserRightsIsActual"].ToString(), out result);
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["UserRightsIsActual"] = value;
            }
        }
    }
}
