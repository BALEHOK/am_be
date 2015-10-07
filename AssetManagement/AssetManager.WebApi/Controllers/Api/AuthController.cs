using System;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using AssetManager.Infrastructure.Models;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        [Route(""), HttpGet]
        public User GetUserInfo()
        {
            var identity = User.Identity as ClaimsIdentity;            
            var user = new  User
            {
                UserName = identity.Name,
                LastLogin = DateTime.Now, //DateTime.Parse(claimsIdentity.FindFirst("LastLogin").Value),
                //Email = claimsIdentity.FindFirst("email").Value
                UserRights = identity.FindFirst("UserRights").Value,
                UserRole = identity.FindFirst("UserRole").Value,
            };

            return user;
        }

        [Route("logout"), HttpGet]
        public void Logout()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();
        }
    }
}