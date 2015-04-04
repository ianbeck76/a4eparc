using System.Web.Mvc;

namespace A4EPARC.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View(GetCompanyId());
        }

    }
}
