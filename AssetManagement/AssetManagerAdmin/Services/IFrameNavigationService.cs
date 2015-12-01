using GalaSoft.MvvmLight.Views;

namespace AssetManagerAdmin.Services
{
    public interface IFrameNavigationService : INavigationService
    {
        object Parameter { get; }
    }
}