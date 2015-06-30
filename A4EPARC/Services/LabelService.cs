using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using A4EPARC.Models;
using A4EPARC.Constants;
using A4EPARC.Models;
using A4EPARC.Repositories;
using A4EPARC.ViewModels;

namespace A4EPARC.Services
{
    public static class LabelService
    {
        public static string Get(string key, string value, IEnumerable<SiteLabelsViewModel> labels, int schemeId)
        {
            schemeId = schemeId == 0 ? 1 : schemeId;
            var languageCode = LanguageCode;
            var label = labels.ToList().FirstOrDefault(l => l.SchemeId == schemeId && l.LanguageCode == languageCode && l.Name == key);
            return label == null ? value : string.IsNullOrEmpty(label.Description) ? labels.ToList().FirstOrDefault(l => l.SchemeId == 1 && l.LanguageCode == "en-GB" && l.Name == key).Description : label.Description;
        }

        public static string LanguageCode
        {
            get
            {
                var cookieObject = HttpContext.Current.Request.Cookies["default_language"];
                return cookieObject == null ? "en-GB" : string.IsNullOrEmpty(cookieObject.Value) ? "en-GB" : cookieObject.Value;
            }
        }
    }
}