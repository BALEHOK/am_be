using System;
using AssetManagerAdmin.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace AssetManagerAdmin.WebApi
{
    public class AssetsApiManager : IAssetsApiManager, ICleanup
    {
        private readonly IMessenger _messenger;
        private IAssetsApi _assetsApi;

        public AssetsApiManager(IMessenger messenger)
        {
            _messenger = messenger;

            messenger.Register<ServerConfig>(this, AppActions.LogoutDone, server =>
            {
                var disposingApi = _assetsApi as IDisposable;
                _assetsApi = null;

                if (disposingApi != null)
                {
                    disposingApi.Dispose();
                }
            });
        }

        public IAssetsApi GetAssetApi(string baseAddress, UserInfo user)
        {
            // [Aleksandr Shukletsov]
            // it's impossible to have AssetAPIs for different servers/users at the same time by design
            // therefore we have a simple local variable for AssetAPI instance 
            if (_assetsApi == null)
            {
                var locator = (UnityServiceLocator)ServiceLocator.Current;
                _assetsApi = locator.Container.Resolve<IAssetsApi>(
                    new ParameterOverride("baseAddress", baseAddress),
                    new ParameterOverride("user", user));
            }
            return _assetsApi;
        }

        public void Cleanup()
        {
            _messenger.Unregister(this);
        }
    }
}