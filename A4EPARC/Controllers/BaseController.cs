using System;
using System.Web.Mvc;
using A4EPARC.Models;
using A4EPARC.Repositories;
using A4EPARC.Services;
using Ninject;
using A4EPARC.ViewModels;
using A4EPARC.Enums;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Web;

namespace A4EPARC.Controllers
{
    public class BaseController : Controller
    {
        protected string GetParameterValue(string controllername, string parametername, string parmaterdata)
        {
            var cookiename = controllername + parametername + "cookie";

            var cookie = Request.Cookies[cookiename];

            if (Request.Cookies[cookiename] == null)
            {
                cookie = new HttpCookie(cookiename);
            }

            if (parmaterdata == null)
            {
                if (cookie != null)
                {
                    var getcookie = Request.Cookies[cookiename];
                    if (getcookie != null)
                    {
                        parmaterdata = getcookie.Value;
                    }
                }
            }

            if (parmaterdata == "null")
            {
                parmaterdata = "";
                Response.Cookies[cookiename].Expires = DateTime.Now.AddDays(-1);
            }

            if (!string.IsNullOrEmpty(parmaterdata))
            {
                cookie.Value = parmaterdata.ToString();
                cookie.Expires = DateTime.Now.AddYears(1);
                Response.Cookies.Add(cookie);
            }
            return parmaterdata;
        }

        protected DateTime? ConvertMinDate(string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                try
                {
                    return DateTime.ParseExact(date, "dd-MM-yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
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
                    var maxdate = DateTime.ParseExact(date, "dd-MM-yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
                    return maxdate.AddDays(1).AddSeconds(-1);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        protected static string ObjectToCsvData<T>(List<T> objects)
        {
            var sb = new StringBuilder();
            Type t = typeof(T);
            PropertyInfo[] pi = t.GetProperties();
            for (int index = 0; index < pi.Length; index++)
            {
                var value = pi[index].Name;
                sb.Append(value.ToString().Contains(",") ? @"""" + value.ToString() + @"""" : value);
                if (index < pi.Length - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append(Environment.NewLine);

            foreach (var obj in objects)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException("obj", "Value can not be null or Nothing!");
                }

                for (int index = 0; index < pi.Length; index++)
                {
                    //sb.Append(@"=""" + pi[index].GetValue(obj, null)+ @"""");
                    var value = pi[index].GetValue(obj, null);

                    sb.Append(value != null && value.ToString().Contains(",") ? @"""" + value.ToString() + @"""" : value);

                    if (index < pi.Length - 1)
                    {
                        sb.Append(",");
                    }
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
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
