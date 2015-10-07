using System.Web.Mvc;
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
            return View();
        }

        // POST: RecoverPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(RecoverPasswordModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username))
            {
                return View((object) "Username is required");
            }

            var user = _userManager.GetUser(model.Username);
            if (user == null)
            {
                return View((object)("User " + model.Username + " not found"));
            }

            var newPassword = user.ResetPassword();

            return _mailService.SendForgotPasswordMail(user.Email, model.Username, newPassword)
                ? View("SuccessfulRecover")
                : View((object) ("Failed to send email to user specified"));
        }
    }
}