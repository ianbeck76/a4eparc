using System;
using System.Linq;
using System.Web.Mvc;
using A4EPARC.Repositories;
using A4EPARC.ViewModels;
using System.Collections.Generic;

namespace A4EPARC.Controllers
{
    [Authorize(Roles = "IsAdmin")]
    public class SiteLabelsController : Controller
    {
        private readonly ISiteLabelsRepository _siteLabelsRepository;
        
        public SiteLabelsController(ISiteLabelsRepository siteLabelsRepository)
        {
            _siteLabelsRepository = siteLabelsRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetRows(string name, int jtStartIndex, int jtPageSize, string jtSorting)
        {
            try
            {
                var list = _siteLabelsRepository.GetJtableView();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    list = list.Where(s => s.Name.StartsWith(name)).ToList();
                }

                var models = list.Select(item => new SiteLabelsViewModel
                {
                    Id = item.Id,
                    LanguageCode = item.LanguageCode,
                    SchemeId = item.SchemeId,
                    Description = item.Description,
                    Name = item.Name,
                    StartIndex = jtStartIndex,
                    Sorting = jtSorting
                }).ToList();

                return Json(new { Result = "OK", Records = models.Skip(jtStartIndex).Take(jtPageSize).ToList(), TotalRecordCount = models.Count() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        [HttpPost]
        public JsonResult AddRow(SiteLabelsViewModel model)
        {
            try
            {
                if (_siteLabelsRepository.SingleOrDefault("SchemeId = @SchemeId AND Name = @Name", new {model.SchemeId, model.Name}) != null)
                    return Json(new {Result = "Error", Message = "Must be unique"});

                model.LanguageCode = "en-GB";
                    _siteLabelsRepository.Add(model);

                    return Json(new
                                    {
                                        Result = "OK",
                                        Record = model
                                    });
            }
            catch (Exception ex)
            {
                return Json(new {Result = "ERROR", ex.Message});
            }
        }

        public JsonResult EditRow(SiteLabelsViewModel model)
        {
            var label = _siteLabelsRepository.SingleOrDefault(model.Id);
            label.Name = model.Name;
            label.Description = model.Description;
            _siteLabelsRepository.Save(label);
            var returnValue = label;
            return Json(new { Result = "OK", Record = returnValue });
        }

        [HttpGet]
        public JsonResult Get(int? schemeId, string languageCode)
        {
            var labels = new List<KeyValuePair<string,string>>();
            var list = _siteLabelsRepository.Get(schemeId.GetValueOrDefault(), languageCode);
            if (list.Any())
            {
                labels = list.Select(s => new KeyValuePair<string, string>(s.Name, s.Description)).ToList();
            }
            else
            {
                labels = _siteLabelsRepository.Get(schemeId.GetValueOrDefault(), languageCode).Select(q => new KeyValuePair<string, string>(q.Name, q.Description)).ToList();
            }
            return Json(new { Labels = labels }, JsonRequestBehavior.AllowGet);
        }
    }
}
