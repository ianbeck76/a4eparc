using A4EPARC.Repositories;
using A4EPARC.Services;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using System.Collections.Generic;
using System;

namespace A4EPARC.Controllers
{
    [Authorize(Roles = "IsSuperAdmin")]
    public class SelectController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;

        public SelectController(IUserRepository userRepository, 
            ICompanyRepository companyRepository)
            : base()
        {
            _companyRepository = companyRepository;
            _userRepository = userRepository;
        }

        public ActionResult Company()
        {
            var companyNames = _companyRepository.All().Select(c => new KeyValuePair<int, string>(c.Id, c.Name)).ToList();
            return View(companyNames);
        }

        [HttpPost]
        public ActionResult Company(string CompanySelected)
        {
            var user = _userRepository.SingleOrDefault(GetLoggedInId());
            user.CompanyId = Convert.ToInt32(CompanySelected);
            _userRepository.Save(user);
            FormsAuthentication.SignOut();
            var showLanguages = _companyRepository.GetLanguages().Where(c => c.CompanyId == user.CompanyId).Count() > 1;
            AuthenticationService.Login(user, showLanguages);

            return RedirectToAction("Index", "Home");
        }

    }
}
