using A4EPARC.Repositories;
using A4EPARC.ViewModels;
using System.Web.Mvc;

namespace A4EPARC.Controllers
{
    public class HomeController : AuthBaseController
    {
        public ActionResult Index()
        {
            var viewmodel = new ClientViewModel();

            viewmodel.SiteLabels = new SiteLabelsRepository().All();

            return View(viewmodel);
        }
    }
}
