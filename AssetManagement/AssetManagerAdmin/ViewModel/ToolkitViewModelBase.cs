using AssetManagerAdmin.Model;
using AssetManagerAdmin.WebApi;
using GalaSoft.MvvmLight;
using Microsoft.Practices.Unity;
using System;

namespace AssetManagerAdmin.ViewModel
{
    public abstract class ToolkitViewModelBase : ViewModelBase
    {
        protected ServerConfig CurrentServer { get; set; }

        protected UserInfo CurrentUser { get; set; }

        protected Action<LoginDoneModel> OnLoginDone { get; set; }

        protected Action OnViewActivated { get; set; }

        [Dependency]
        public IAssetsApiManager AssetsApiManager { get; set; }

        public IAssetsApi Api
        {
            get
            {
                if (AssetsApiManager == null)
                    throw new NullReferenceException("AssetsApiManager");
                if (CurrentServer == null)
                    throw new NullReferenceException("CurrentServer");
                if (CurrentUser == null)
                    throw new NullReferenceException("CurrentUser");
                return AssetsApiManager.GetAssetApi(CurrentServer.ApiUrl, CurrentUser);
            }
        }

        public ToolkitViewModelBase()
        {
            MessengerInstance.Register<LoginDoneModel>(this, AppActions.LoginDone, model =>
            {
                CurrentServer = model.Server;
                CurrentUser = model.User;
                if (OnLoginDone != null)
                    OnLoginDone.Invoke(model);
            });

            MessengerInstance.Register<Type>(this, AppActions.DataContextChanged, currentView =>
            {
                if (currentView == GetType())
                {
                    if (OnViewActivated != null)
                        OnViewActivated.Invoke();
                }
            });
        }
    }
}
