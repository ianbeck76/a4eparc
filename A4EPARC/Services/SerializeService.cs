using System.IO;
using System.Web.Mvc;

namespace A4EPARC.Services
{
public class SerializeService :  Controller, ISerializeService
    {
        public string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model)
        {
            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var viewContext = new ViewContext(controllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public string RenderRazorViewToString(ControllerContext controllerContext, string viewName)
        {
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var viewContext = new ViewContext(controllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public string RenderRazorViewToString(ControllerContext controllerContext, string viewName, ViewDataDictionary viewData)
        {
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var viewContext = new ViewContext(controllerContext, viewResult.View, viewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }

    public interface ISerializeService
    {
        string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model);
        string RenderRazorViewToString(ControllerContext controllerContext, string viewName, ViewDataDictionary viewData);
    }
}