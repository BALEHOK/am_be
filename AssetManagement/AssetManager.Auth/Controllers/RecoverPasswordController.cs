using System.Web.Mvc;
using AppFramework.Auth;
using AppFramework.Auth.Users;
using AssetManager.Auth.Email;
using AssetManager.Auth.Models;

namespace AssetManager.Auth.Controllers
{
    [AllowAnonymous]
    public class RecoverPasswordController : Controller
    {
        private readonly IUserManager _userManager;
        private readonly IMailService _mailService;

        public RecoverPasswordController()
        {
            _userManager = new UserManager();
            _mailService = new AuthMailService();
        }

        // GET: RecoverPassword
        public ActionResult Index()
        {
            return View(new RecoverPasswordPageModel
            {
                LinkToLogin = GetLoginPageUrl()
            });
        }

        // POST: RecoverPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(RecoverPasswordModel model)
        {
            var pageModel = new RecoverPasswordPageModel
            {
                LinkToLogin = GetLoginPageUrl()
            };

            if (string.IsNullOrWhiteSpace(model.Username))
            {
                pageModel.Error = "Username is required";
                return View(pageModel);
            }

            var user = _userManager.GetUser(model.Username);
            if (user == null)
            {
                pageModel.Error = "User \"" + model.Username + "\" not found";
                return View(pageModel);
            }

            var newPassword = user.ResetPassword();

            if (_mailService.SendForgotPasswordMail(user.Email, model.Username, newPassword))
            {
                return View("SuccessfulRecover", pageModel);
            }

            pageModel.Error = "Failed to send email to user specified";
            return View(pageModel);
        }

        private string GetLoginPageUrl()
        {
            var signin = Request.Params["signin"];
            if (!string.IsNullOrEmpty(signin))
            {
                return Startup.AuthRoot + "/" + AuthConstants.RoutePaths.Login
                       + "?signin=" + signin;
            }

            return string.Empty;
        }
    }
}