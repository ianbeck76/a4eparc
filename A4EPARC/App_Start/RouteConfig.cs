using System.Web.Mvc;
using System.Web.Routing;

namespace A4EPARC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
              name: "Home",
              url: "{controller}/{action}/{id}",
              defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "ExportList",
                url: "Results/ExportList/{datefrom}/" +
                  "{dateto}/" +
                  "{jobseekerid}/" +
                  "{surname}/" +
                  "{username}/" +
                  "{company}",
                defaults: new
                              {
                                  controller = "Results", 
                                  action = "ExportList", 
                                  datefrom = UrlParameter.Optional, 
                                  dateto = UrlParameter.Optional,
                                  jobseekerid = UrlParameter.Optional,
                                  surname = UrlParameter.Optional,
                                  username = UrlParameter.Optional,
                                  company = UrlParameter.Optional   
                              });

            routes.MapRoute(
             name: "results",
             url: "Results/" +
                  "{datefrom}/" +
                  "{dateto}/" +
                  "{jobseekerid}/" +
                  "{surname}/" +
                  "{username}/" +
                  "{company}",
             defaults: new
             {
                 controller = "Results",
                 action = "Index",
                 datefrom = UrlParameter.Optional,
                 dateto = UrlParameter.Optional,
                 jobseekerid = UrlParameter.Optional,
                 surname = UrlParameter.Optional,
                 username = UrlParameter.Optional,
                 company = UrlParameter.Optional   
             });

            routes.MapRoute(
            name: "webserviceresults",
            url: "WebServiceResults/Index/" +
                 "{datefrom}/" +
                 "{dateto}/" +
                 "{jobseekerid}/" +
                 "{environment}/" +
                 "{company}",
            defaults: new
            {
                controller = "WebServiceResults",
                action = "Index",
                datefrom = UrlParameter.Optional,
                dateto = UrlParameter.Optional,
                jobseekerid = UrlParameter.Optional,
                environment = UrlParameter.Optional,
                company = UrlParameter.Optional
            });
        }
    }
}