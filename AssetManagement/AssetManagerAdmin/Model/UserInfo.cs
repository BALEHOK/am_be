using AssetManager.Infrastructure.Models;

namespace AssetManagerAdmin.Model
{
    public class UserInfo
    {        
        public string Password { get; private set; }
        public string IdToken { get; private set; }
        public string AccessToken { get; private set; }
        public User UserModel { get; private set; }

        public UserInfo(User userModel, string accessToken, string idToken)
        {
            UserModel = userModel;
            AccessToken = accessToken;
            IdToken = idToken;
        }
    }
}