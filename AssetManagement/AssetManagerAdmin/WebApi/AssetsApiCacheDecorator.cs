using System;
using System.Runtime.Caching;
using System.Threading.Tasks;
using AssetManager.Infrastructure.Models.TypeModels;
using AssetManagerAdmin.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace AssetManagerAdmin.WebApi
{
    public class AssetsApiCacheDecorator : AssetsApi, ICleanup, IDisposable
    {
        private readonly IMessenger _messenger;
        private const string TypesInfoModelKey = "TypesInfoModel";
        private readonly ObjectCache _cache = MemoryCache.Default;

        private bool _disposed;

        public AssetsApiCacheDecorator(string baseAddress, UserInfo user)
            : this(baseAddress, user, Messenger.Default)
        {
        }

        public AssetsApiCacheDecorator(string baseAddress, UserInfo user, IMessenger messenger)
            : base(baseAddress, user)
        {
            _messenger = messenger;

            messenger.Register<object>(this, AppActions.ClearTypesInfoCache, x => CacheRemove(TypesInfoModelKey));
        }

        public override async Task<TypesInfoModel> GetTypesInfo()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("AssetsApiCacheDecorator");
            }

            var typesInfo = CacheGet<TypesInfoModel>(TypesInfoModelKey);
            if (typesInfo != null)
            {
                return typesInfo;
            }

            typesInfo = await base.GetTypesInfo();

            CacheSet(TypesInfoModelKey, 60, typesInfo);

            return typesInfo;
        }

        private T CacheGet<T>(string key)
            where T : class
        {
            return _cache.Get(key) as T;
        }

        private void CacheSet<T>(string key, int expiration, T value)
            where T : class
        {
            _cache.Set(key, value, DateTimeOffset.Now.AddMinutes(expiration));
        }

        private void CacheRemove(string key)
        {
            _cache.Remove(key);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _disposed = true;
                CacheRemove(TypesInfoModelKey);
            }

            Cleanup();
        }

        public void Cleanup()
        {
            _messenger.Unregister(this);
        }
    }
}