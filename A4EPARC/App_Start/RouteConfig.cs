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
                name: "PrintList",
                url: "Results/PrintList/{datefrom}/" +
                  "{dateto}/" +
                  "{caseid}/",
                defaults: new
                              {
                                  controller = "Results", 
                                  action = "PrintList", 
                                  datefrom = UrlParameter.Optional, 
                                  dateto = UrlParameter.Optional,
                                  caseid = UrlParameter.Optional
                              }
);

            routes.MapRoute(
               name: "Home",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
             name: "results",
             url: "Results/" +
                  "{datefrom}/" +
                  "{dateto}/" +
                  "{caseid}/" +
                  "{clientid}/",
             defaults: new
             {
                 controller = "Results",
                 action = "Index",
                 datefrom = UrlParameter.Optional,
                 dateto = UrlParameter.Optional,
                 caseid = UrlParameter.Optional,
                 clientid = UrlParameter.Optional             
             });

        }
    }
}