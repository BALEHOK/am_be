using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.Core.Interfaces;
using AppFramework.Core.PL;
using AssetSite.Controls;
using Microsoft.Practices.Unity;

namespace AssetSite
{
    public class UnityHttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
        }

        public void Dispose()
        {
        }
        
        private void OnPreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (HttpContext.Current.Handler == null)
            {
                return;
            }
            
            var page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                var app = (IHttpUnityApplication)HttpContext.Current.ApplicationInstance;
                PageInitializer.HookEventsForUserControlInitialization(page, app.UnityContainer);
            }
        }
    }

    internal sealed class PageInitializer
    {
        private readonly Page _page;
        private readonly IUnityContainer _container;
        private readonly ContolDependencyResolver _resolver;

        internal PageInitializer(Page page, IUnityContainer container)
        {
            if (page == null)
                throw new ArgumentNullException("page");
            if (container == null)
                throw new ArgumentNullException("container");
            _page = page;
            _container = container;
            _resolver = new ContolDependencyResolver(_container);
        }

        internal static void HookEventsForUserControlInitialization(Page page, IUnityContainer container)
        {
            var initializer = new PageInitializer(page, container);
            page.PreInit += initializer.PreInit;
            page.InitComplete += initializer.InitComplete;
        }

        private void PreInit(object sender, EventArgs e)
        {
            _container.BuildUp(Roles.Provider.GetType(), Roles.Provider);
            _container.BuildUp(Membership.Provider.GetType(), Membership.Provider);
            RecursivelyInitializeMasterPages();
        }

        private void RecursivelyInitializeMasterPages()
        {
            foreach (var masterPage in GetMasterPages())
                _container.BuildUp(masterPage.GetType(), masterPage);
        }

        private IEnumerable<MasterPage> GetMasterPages()
        {
            MasterPage master = this._page.Master;
            while (master != null)
            {
                yield return master;
                master = master.Master;
            }
        }

        private void InitComplete(object sender, EventArgs e)
        {
            // Base page provides some common services via protected properties
            if (_page as BasePage != null)
                _container.BuildUp((BasePage)_page);
            _resolver.InitializeControlTree(_page);
        }
    }
}
