using IdentityServer3.Core;

namespace AppFramework.Auth
{
    public static class AuthConstants
    {
        public static string SiteName = "Asset management authentication";

        public static class Scopes
        {
            public const string OpenId = Constants.StandardScopes.OpenId;
            public const string WebApi = "webApi";
            public const string Profile = "profile";
        }

        public static class ClaimTypes
        {
            public const string Subject = Constants.ClaimTypes.Subject;
            public const string UserName = "userName";
            public const string Email = Constants.ClaimTypes.Email;
            public const string Role = Constants.ClaimTypes.Role;
            public const string UserRights = "userRights";
            public const string UserRole = "userRole";
            public const string LastLogin = "lastLogin";
        }

        public static class RoutePaths
        {
            public const string Welcome = Constants.RoutePaths.Welcome;
            public const string Login = Constants.RoutePaths.Login;
            public const string LoginExternal = Constants.RoutePaths.LoginExternal;
            public const string LoginExternalCallback = Constants.RoutePaths.LoginExternalCallback;
            public const string Logout = Constants.RoutePaths.Logout;
            public const string ResumeLoginFromRedirect = Constants.RoutePaths.ResumeLoginFromRedirect;
            public const string CspReport = Constants.RoutePaths.CspReport;
            public const string ClientPermissions = Constants.RoutePaths.ClientPermissions;
        }

        public static class Endpoints
        {
            public const string Authorize = "/connect/authorize";
            public const string Logout = "/connect/endsession";
            public const string Token = "/connect/token";
            public const string UserInfo = "/connect/userinfo";
            public const string IdentityTokenValidation = "/connect/identitytokenvalidation";
            public const string TokenRevocation = "/connect/revocation";
        }


    }
}