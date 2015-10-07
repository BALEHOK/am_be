using System;
using System.Web;

/// <summary>
/// Implements ASP unhandled exception manager as a HttpModule
/// </summary>
/// <remarks>
/// to use:
///    1) place dll in the \bin folder
///    2) add this section to your Web.config under the <system.web> element:
///         <httpModules>
///	 	        <add name="ASPUnhandledException" 
///                 type="AppFramework.CustomException.CustomExHandlerHttpModule, AppFramework.CustomException" />
///		    </httpModules>
///
///  Jeff Atwood
///  http://www.codinghorror.com/
/// </remarks>

namespace AppFramework.CustomException
{
    public class CustomExHandlerHttpModule : IHttpModule
    {

        void System.Web.IHttpModule.Init(System.Web.HttpApplication Application)
        {
            Application.Error += OnError;
        }

        void System.Web.IHttpModule.Dispose()
        {
        }

        protected virtual void OnError(object sender, EventArgs args)
        {
            HttpApplication app = (HttpApplication)sender;
            Handler ueh = new Handler();
            ueh.HandleException(app.Server.GetLastError());
        }
    }
}