using System;
using System.Web.Mvc;
using A4EPARC.Models;
using A4EPARC.Repositories;
using A4EPARC.Services;
using Ninject;
using A4EPARC.ViewModels;
using A4EPARC.Enums;
using System.Linq;

namespace A4EPARC.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected DateTime? ConvertMinDate(string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                try
                {
                    return Convert.ToDateTime(date);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        protected DateTime? ConvertMaxDate(string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                try
                {
                    return Convert.ToDateTime(date).AddDays(1).AddSeconds(-1);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        protected ClientViewModel AppendActionDetailsToViewModel(ClientViewModel viewmodel)
        {
            if (viewmodel.ActionIdToDisplay != (int)ActionType.Undefined)
            {
                viewmodel.ActionTypeName = Enum.GetName(typeof(ActionType), viewmodel.ActionIdToDisplay);
            }
            return viewmodel;
        }

        [Inject]
        public IUserRepository UserRepository { get; set; }

        public User GetCurrentUser()
        {
            if (HttpContext.Items["CurrentUser" + User.Identity.Name] == null)
            {
                var user = UserRepository.SingleOrDefault("Id = @Id", new { Id = GetLoggedInId() });
                HttpContext.Items["CurrentUser" + User.Identity.Name] = user;
            }
            return (User)HttpContext.Items["CurrentUser" + User.Identity.Name];
        }

        public int GetCompanyId()
        {
            return AuthenticationService.GetCompanyId();
        }

        [Inject]
        public ICompanyRepository CompanyRepository { get; set; }

        public Company GetCompanyDetails() 
        {
            var companyId = GetCompanyId();
            if (companyId == 0)
            {
                if (Request.Url.AbsoluteUri.Contains("gls") || Request.Url.AbsoluteUri.Contains("ac3"))
                {
                    companyId = 11;
                }
                else if (Request.Url.AbsoluteUri.Contains("a4e"))
                {
                    companyId = 5;
                }
                else if (Request.Url.AbsoluteUri.Contains("exemplar"))
                {
                    companyId = 10;
                }
                else 
                {
                    companyId = 1;
                }
            }
            return CompanyRepository.SingleOrDefault("Id = @Id", new { Id = companyId });
        }

        public int GetLoggedInId()
        {
            return AuthenticationService.GetLoggedInId();
        }

        public string GetLoggedInEmail()
        {
            return AuthenticationService.GetLoggedInEmail();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.BrowseResultsLink = GetBrowseResultsLink();
        }

        public string GetBrowseResultsLink()
        {
            var reportDateFrom = "";//GetCookieValue(CookieName, GetCookieFilterKey(product, CookieFilters.ReportDateFrom));
            var reportDateTo = "";//GetCookieValue(CookieName, GetCookieFilterKey(product, CookieFilters.ReportDateTo));
            var jobseekerid = "";//GetCookieValue(CookieName, GetCookieFilterKey(product, CookieFilters.Reference));
            var surname = "";//GetCookieValue(CookieName, GetCookieFilterKey(product, CookieFilters.RegistrationNumber));

            if (string.IsNullOrEmpty(reportDateFrom))
                reportDateFrom = "null";
            if (string.IsNullOrEmpty(reportDateTo))
                reportDateTo = "null";
            if (string.IsNullOrEmpty(jobseekerid))
                jobseekerid = "null";
            if (string.IsNullOrEmpty(surname))
                surname = "null";
            var l = ("/results/" + reportDateFrom + "/" + reportDateTo + "/" + jobseekerid + "/" + surname);
            return l;
        }

        public string GetEmptyBrowseResultsLink()
        {
            return "/results/null/null/null/null";
        }

        public string LanguageCode 
        {
            get 
            {
                var cookieObject = Request.Cookies["default_language"];
                return  cookieObject == null ? "en-GB" : cookieObject.Value;
            }
        }
    }
}
