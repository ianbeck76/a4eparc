using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using A4EPARC.Repositories;
using A4EPARC.Services;
using A4EPARC.Models;

namespace A4EPARC.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ISerializeService _serializeService;

        public AccountController(IAuthenticationService authenticationService, 
            IUserRepository userRepository,
            IEmailService emailService, ISerializeService serializeService)
            : base()
        {
            _authenticationService = authenticationService;
            _userRepository = userRepository;
            _emailService = emailService;
            _serializeService = serializeService;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var userId = _authenticationService.Login(model.Email, model.Password);
                if (userId != 0)
                {
                    var user = _userRepository.SingleOrDefault(userId);
                    if (user.IsSuperAdmin)
                    {
                        return RedirectToAction("Company", "Select");
                    }
                    return RedirectToLocal(returnUrl);
                }
            }
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgottenPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgottenPassword(ForgottenPasswordModel forgottenPasswordModel)
        {
            if (ModelState.IsValid)
            {
                if (_authenticationService.LoginExists(forgottenPasswordModel.Email))
                {
                    var company = GetCompanyDetails();
                    try
                    {
                        var html = _serializeService.RenderRazorViewToString(this.ControllerContext,
                                                                            "_ForgottenPasswordEmail",
                                                                            new PasswordViewModel
                                                                            {
                                                                                Username =
                                                                                    forgottenPasswordModel.Email,
                                                                                Password = company.DefaultPassword
                                                                            });
                        _authenticationService.ChangePassword(forgottenPasswordModel.Email, company.DefaultPassword);
                        _emailService.Send(company.EmailFromAddress,
                    forgottenPasswordModel.Email, "Forgotten Password", html);
  
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("ForgottenPasswordFailure");
                    }
                    return RedirectToAction("ForgottenPasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("EmailDoesntExist", "Email does not exist.");
                }
            }
            return View(forgottenPasswordModel);
        }

        public ActionResult ForgottenPasswordSuccess()
        {
            return View();
        }
        public ActionResult ForgottenPasswordFailure()
        {
            return View();
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
//            User user = _userRepository.SingleOrDefault("email=@Email", new { email });
            //if (user != null && user.ChangePassword.HasValue && login.ChangePassword.Value)
            //{
             //   ViewBag.ChangePassword = true;
               // return View();
            //}
            return View();
        }

        public User ValidatePassword(string email, string password)
        {
            User user = _userRepository.SingleOrDefault("email=@Email", new { email });
            if (((user != null)))
            {
                string saltedPassword = password + user.Salt;
                string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(saltedPassword, "SHA1");

                if ((user.Password == hash))
                {
                    //user.FailedLogins = 0;
                    //_userRepository.SaveChanges();
                    return user;
                }
                else
                {
 //                   login.FailedLogins = login.FailedLogins == null ? (short?)1 : (short?)(login.FailedLogins.Value + (short?)1);
                    //if (login.FailedLogins >2)
                    //    login.ChangePassword = true;
   //                 _loginRepository.SaveChanges();
                }
            }
            return null;
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.NewPassword.ToLower() == "password")
                {
                    ModelState.AddModelError("", "The new password is invalid.");
                    return View(model);
                }

                bool changePasswordSucceeded = false;
                try
                {
                    var user = _authenticationService.ValidatePassword(AuthenticationService.GetLoggedInEmail(), model.OldPassword);
                    if (user != null)
                    {
                        _authenticationService.ChangePassword(user.Email, model.NewPassword);
                        changePasswordSucceeded = true;
                    }
                }
                catch (Exception ex)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }
            return View(model);
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
    }
}
