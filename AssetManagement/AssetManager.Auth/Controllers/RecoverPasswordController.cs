using System.Web.Mvc;
using AppFramework.Auth;
using AppFramework.Auth.Users;
using AppFramework.Email.Services;
using AssetManager.Auth.Models;
using AppFramework.Email;

namespace AssetManager.Auth.Controllers
{
    [AllowAnonymous]
    public class RecoverPasswordController : Controller
    {
        private readonly IUserManager _userManager;
        private readonly IEmailService _mailService;

        public RecoverPasswordController()
        {
            _userManager = new UserManager();
            _mailService = new EmailService(new ViewLoader());
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

            _mailService.SendForgotPasswordMail(
                user.Email, 
                model.Username,
                user.ResetPassword());

            return View("SuccessfulRecover", pageModel);
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