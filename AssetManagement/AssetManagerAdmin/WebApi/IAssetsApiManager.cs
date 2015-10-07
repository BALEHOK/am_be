using AssetManagerAdmin.Model;

namespace AssetManagerAdmin.WebApi
{
    public interface IAssetsApiManager
    {
        IAssetsApi GetAssetApi(string baseAddress, UserInfo user);
    }
}