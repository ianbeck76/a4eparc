using System;
using System.Web.Mvc;
using A4EPARC.Models;
using A4EPARC.Repositories;
using A4EPARC.Services;
using Ninject;

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
            var caseId = "";//GetCookieValue(CookieName, GetCookieFilterKey(product, CookieFilters.Reference));
            var clientId = "";//GetCookieValue(CookieName, GetCookieFilterKey(product, CookieFilters.RegistrationNumber));

            if (string.IsNullOrEmpty(reportDateFrom))
                reportDateFrom = "null";
            if (string.IsNullOrEmpty(reportDateTo))
                reportDateTo = "null";
            if (string.IsNullOrEmpty(caseId))
                caseId = "null";
            if (string.IsNullOrEmpty(clientId))
                clientId = "null";
            var l = ("/results/" + reportDateFrom + "/" + reportDateTo + "/" + caseId + "/" + clientId);
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
