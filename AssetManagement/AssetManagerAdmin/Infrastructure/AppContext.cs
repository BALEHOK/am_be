using AssetManagerAdmin.Model;

namespace AssetManagerAdmin.Infrastructure
{
    public class AppContext : IAppContext
    {
        public UserInfo CurrentUser { get; private set; }

        public ServerConfig CurrentServer { get; private set; }

        public void SetUser(UserInfo user)
        {
            CurrentUser = user;
        }

        public void SetServer(ServerConfig server)
        {
            CurrentServer = server;
        }
    }
}
