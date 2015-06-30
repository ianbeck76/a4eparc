using System.Web.Mvc;

namespace A4EPARC.Controllers
{
    public class HomeController : AuthBaseController
    {
        public ActionResult Index()
        {
            return View(GetCompanyId());
        }
    }
}
