using System;
using System.Web.Http;
using AppFramework.Core.Classes;
using AssetManager.Infrastructure.Models;
using AppFramework.Email.Services;
using AssetManager.Infrastructure.Extensions;

namespace AssetManager.WebApi.Controllers.Api
{
    public class ContactsController : ApiController
    {
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public ContactsController(IEmailService emailService, IUserService userService)
        {
            if (emailService == null)
                throw new ArgumentNullException("emailService");
            _emailService = emailService;
            if (userService == null)
                throw new ArgumentNullException("userService");
            _userService = userService;
        }

        // POST api/contacts
        public IHttpActionResult Post(ContactFormMessage message)
        {
            var userId = User.GetId();
            var user = _userService.GetById(userId);
            _emailService.SendContactFormEmail(user.UserName, user.Email, message.Subject, message.Message);
            return Ok();
        }
    }
}