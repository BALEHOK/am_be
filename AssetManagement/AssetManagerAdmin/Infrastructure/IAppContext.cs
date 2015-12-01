using AssetManagerAdmin.Model;

namespace AssetManagerAdmin.Infrastructure
{
    public interface IAppContext
    {
        ServerConfig CurrentServer { get; }
        UserInfo CurrentUser { get; }

        void SetServer(ServerConfig server);
        void SetUser(UserInfo user);
    }
}