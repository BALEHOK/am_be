using AssetManagerAdmin.Infrastructure;
using AssetManagerAdmin.Model;
using AssetManagerAdmin.Services;
using AssetManagerAdmin.WebApi;
using Common.Logging;
using GalaSoft.MvvmLight;
using Microsoft.Practices.Unity;
using System;

namespace AssetManagerAdmin.ViewModels
{
    public abstract class ToolkitViewModelBase : ViewModelBase
    {
        public event Action<object> OnNavigated;

        [Dependency]
        public ILog Logger { get; set; }

        [Dependency]
        public IFrameNavigationService NavigationService { get; set; }

        [Dependency]
        public IAssetsApiManager AssetsApiManager { get; set; }

        public IAppContext Context { get; private set; }

        public IAssetsApi Api
        {
            get
            {
                if (AssetsApiManager == null)
                    throw new NullReferenceException("AssetsApiManager");
                if (Context == null)
                    throw new NullReferenceException("Context");
                if (Context.CurrentServer == null)
                    throw new NullReferenceException("CurrentServer");
                if (Context.CurrentUser == null)
                    throw new NullReferenceException("CurrentUser");

                return AssetsApiManager.GetAssetApi(
                    Context.CurrentServer.ApiUrl, 
                    Context.CurrentUser);
            }
        }

        public ToolkitViewModelBase(IAppContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            Context = context;

            MessengerInstance.Register<LoginDoneModel>(this, AppActions.LoginDone, model =>
            {
                Context.SetServer(model.Server);
                Context.SetUser(model.User);
            });
        }

        public void RaiseNavigatedEvent()
        {
            if (OnNavigated != null)
                OnNavigated(NavigationService.Parameter);
        }
    }
}
