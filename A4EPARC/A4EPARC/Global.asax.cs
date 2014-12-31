﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using A4EPARC.App_Start;

namespace A4EPARC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            CacheConfig.Initialise();
        }

        protected void Application_AuthenticateRequest()
        {
            if (HttpContext.Current.User == null)
            {
                return;
            }
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return;
            }
            if (!(HttpContext.Current.User.Identity is FormsIdentity))
            {
                return;
            }

            var id = HttpContext.Current.User.Identity as FormsIdentity;
            var ticket = id.Ticket;
            var userData = ticket.UserData;
            var roles = userData.Split(Convert.ToChar(","));

            HttpContext.Current.User = new GenericPrincipal(id, roles);
        }
    }
}