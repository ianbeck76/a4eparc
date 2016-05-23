using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using A4EPARC.Controllers;
using A4EPARC.Extensions;
using A4EPARC.Models;
using A4EPARC.Repositories;
using A4EPARC.Services;
using A4EPARC.ViewModels;

namespace A4EPARC.Areas.Admin.Controllers
{

    [Authorize(Roles = "IsAdmin")]
    public class UserController : AuthBaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly ISecurityService _securityService;
        private readonly ICompanyRepository _companyRepository;
        private readonly IEmailRepository _emailRepository;
        private readonly IEmailService _emailService;
        private readonly IAuthenticationService _authenticationService;

        public UserController(ISecurityService securityService,
            IUserRepository userRepository, ICompanyRepository companyRepository,
            IEmailRepository emailRepository, IEmailService emailService,
            IAuthenticationService authenticationService)
        {
            _securityService = securityService;
            _userRepository = userRepository;
            _companyRepository = companyRepository;
            _emailRepository = emailRepository;
            _emailService = emailService;
            _authenticationService = authenticationService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetCompanies()
        {
            var companies = _companyRepository.All().Where(c => c.IsActive == true).Select(c => c.Name);

            return Json(new { Result = "OK", Options = companies });
        }

        [HttpPost]
        public ActionResult GetUsers(string emailaddress, int jtStartIndex, int jtPageSize, string jtSorting)
        {
            try
            {
                var users = _userRepository.GetJtableView();

                var currentCompanyId = GetCurrentUser().CompanyId;
                if (currentCompanyId > 1)
                {
                    users = users.Where(u => u.CompanyId == currentCompanyId).ToList();
                }

                if (!string.IsNullOrWhiteSpace(emailaddress))
                {
                    users = users.Where(u => u.Email.ToLower().StartsWith(emailaddress.ToLower())).ToList();
                }

                #region Ordering

                if (string.IsNullOrEmpty(jtSorting) || jtSorting.Equals("Email ASC"))
                {
                    users = users.OrderBy(u => u.Email).ToList();
                }
                else if (jtSorting.Equals("Email DESC"))
                {
                    users = users.OrderByDescending(u => u.Email).ToList();
                }
                else if (jtSorting.Equals("Company ASC"))
                {
                    users = users.OrderBy(p => p.CompanyName).ToList();
                }
                else if (jtSorting.Equals("Company DESC"))
                {
                    users = users.OrderByDescending(p => p.CompanyName).ToList();
                }
                else if (jtSorting.Equals("IsAdmin ASC"))
                {
                    users = users.OrderBy(u => u.IsAdmin).ToList();
                }
                else if (jtSorting.Equals("IsAdmin DESC"))
                {
                    users = users.OrderByDescending(u => u.IsAdmin).ToList();
                }
                else if (jtSorting.Equals("IsSuperAdmin ASC"))
                {
                    users = users.OrderBy(u => u.IsSuperAdmin).ToList();
                }
                else if (jtSorting.Equals("IsSuperAdmin DESC"))
                {
                    users = users.OrderByDescending(u => u.IsSuperAdmin).ToList();
                }
                else if (jtSorting.Equals("IsViewer ASC"))
                {
                    users = users.OrderBy(p => p.IsViewer).ToList();
                }
                else if (jtSorting.Equals("IsViewer DESC"))
                {
                    users = users.OrderByDescending(p => p.IsViewer).ToList();
                }

                #endregion


                var userViewModels = users.Select(item => new UserViewModel
                {
                    Id = item.Id,
                    Email = item.Email,
                    IsSuperAdmin = item.IsSuperAdmin,
                    IsAdmin = item.IsAdmin,
                    IsViewer = item.IsViewer,
                    CompanyId = item.CompanyId,
                    CompanyName = item.CompanyName,
                    IsActive = item.IsActive,
                    StartIndex = jtStartIndex,
                    Sorting = jtSorting
                }).ToList();

                return Json(new { Result = "OK", Records = userViewModels.Skip(jtStartIndex).Take(jtPageSize).ToList(), TotalRecordCount = userViewModels.Count() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        [HttpPost]
        public JsonResult AddUser(UserViewModel model)
        {
            try
            {
                if (_userRepository.SingleOrDefault("Email = @Email", new { model.Email }) != null)
                    return Json(new { Result = "Error", Message = "User email must be unique" });

                if (Utils.IsValidEmail(model.Email))
                {

                    var companyName = model.CompanyName;

                    if (string.IsNullOrEmpty(companyName))
                    {
                        companyName =
                            _userRepository.GetJtableView()
                                           .FirstOrDefault(u => u.Email.ToLower() == GetCurrentUser().Email.ToLower())
                                           .CompanyName;
                    }

                    var salt = _authenticationService.CreateSalt(16);
                    var saltedPassword = model.ResetPassword + salt;
                    var hash = FormsAuthentication.HashPasswordForStoringInConfigFile(saltedPassword, "SHA1").ToUpper();

                    IUser user = new User
                                     {
                                         Email = model.Email,
                                         Password = hash,
                                         Salt = salt,
                                         IsSuperAdmin = false,
                                         IsAdmin = model.IsAdmin,
                                         IsViewer = model.IsViewer,
                                         IsActive = true,  
                                         CompanyId =
                                             _companyRepository.All().FirstOrDefault(c => c.Name == companyName).Id,
                                         CreatedDate = DateTime.UtcNow
                                     };
                    _userRepository.Add(user);

                    try
                    {
                        var company = GetCompanyDetails();
                        var html = new SerializeService().RenderRazorViewToString(this.ControllerContext,
                                                                            "_NewUserEmail",
                                                                            new PasswordViewModel
                                                                            {
                                                                                Username = user.Email,
                                                                                Password = model.ResetPassword
                                                                            });

                        _emailService.Send(company.EmailFromAddress, user.Email, "New User", html);

                        _emailRepository.InsertEmailLog();
                    }
                    catch (Exception exception)
                    {

                    }

                    return Json(new
                                    {
                                        Result = "OK",
                                        Record = user
                                    });
                }
                else
                {
                    return Json(new { Result = "ERROR", Message = "Invalid Email Address" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        public JsonResult EditUser(UserViewModel model)
        {
            IUser user = new User();

            if (string.IsNullOrEmpty(model.CompanyName))
            {
                var existingUser = _userRepository.GetJtableView().FirstOrDefault(u => u.Id == model.Id);
                user.IsSuperAdmin = existingUser.IsSuperAdmin;
                user.CompanyId = existingUser.CompanyId;
            }
            else
            {
                user.IsSuperAdmin = model.IsSuperAdmin;
                user.CompanyId = _companyRepository.All().FirstOrDefault(c => c.Name == model.CompanyName).Id;
            }

            user.Id = model.Id;
            user.IsAdmin = model.IsAdmin;
            user.IsViewer = model.IsViewer;
            user.IsActive = true;

            _userRepository.Save(user);
            var returnUser = _userRepository.SingleOrDefault(user.Id);
            return Json(new { Result = "OK", Record = returnUser });
        }

        [HttpPost]
        public ActionResult AdminResetPassword(int id)
        {
            var user = _userRepository.SingleOrDefault(id);
            var company = _companyRepository.SingleOrDefault(user.CompanyId);

            var newpassword = Guid.NewGuid().ToString().Substring(0, 8);

            var returnValue = SendResetPasswordEmail(user, company, newpassword);

            return Json(new { IsValid = returnValue, Password = newpassword });
        }

        [HttpPost]
        public ActionResult Deactivate(int id)
        {
            var user = _userRepository.SingleOrDefault(id);

            user.IsActive = false;

            _userRepository.Save(user);

            return Json(new { IsValid = true });
        }

        [HttpPost]
        public ActionResult Reactivate(int id)
        {
            var user = _userRepository.SingleOrDefault(id);

            user.IsActive = true;

            _userRepository.Save(user);

            return Json(new { IsValid = true });
        }


        [HttpPost]
        public ActionResult ResetPassword(int id)
        {
            var user = _userRepository.SingleOrDefault(id);
            var company = GetCompanyDetails();

            var newpassword = Guid.NewGuid().ToString().Substring(0,8);

            var returnValue = SendResetPasswordEmail(user, company, newpassword);

            return Json(new { IsValid = returnValue, Password = newpassword });
        }

        private bool SendResetPasswordEmail(IUser user, ICompany company, string newpassword)
        {
            if (_emailRepository.GetEmailLogCount() < 201)
            {
                try
                {
                    var html = new SerializeService().RenderRazorViewToString(this.ControllerContext,
                                                                        "_ForgottenPasswordEmail",
                                                                        new PasswordViewModel
                                                                        {
                                                                            Username = user.Email,
                                                                            Password = newpassword
                                                                        });
                    _authenticationService.ChangePassword(user.Email, newpassword);

                    _emailService.Send(company.EmailFromAddress, user.Email, "Account Info", html);

                    _emailRepository.InsertEmailLog();
                }
                catch (Exception exception)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

    }
}