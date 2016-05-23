using System;
using System.Linq;
using System.Web.Mvc;
using A4EPARC.Repositories;
using A4EPARC.ViewModels;
using System.Collections.Generic;
using A4EPARC.Services;

namespace A4EPARC.Controllers
{
    public class SiteLabelsController : Controller
    {
        private readonly ISiteLabelsRepository _siteLabelsRepository;
        
        public SiteLabelsController(ISiteLabelsRepository siteLabelsRepository)
        {
            _siteLabelsRepository = siteLabelsRepository;
        }

        [Authorize(Roles = "IsSuperAdmin")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "IsSuperAdmin")]
        public ActionResult GetRows(int? scheme, string language, string name, int jtStartIndex, int jtPageSize, string jtSorting)
        {
            try
            {
                var list = _siteLabelsRepository.GetJtableView().Where(s => s.SchemeId == scheme.GetValueOrDefault() && s.LanguageCode == language).ToList();

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

                return Json(new { Result = "OK", Records = models.Skip(jtStartIndex).Take(jtPageSize).ToList().OrderBy(m => m.Name), TotalRecordCount = models.Count() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        [Authorize(Roles = "IsSuperAdmin")]
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

        [Authorize(Roles = "IsSuperAdmin")]
        public JsonResult EditRow(SiteLabelsViewModel model)
        {
            var label = _siteLabelsRepository.SingleOrDefault(model.Id);
            label.Description = model.Description;
            _siteLabelsRepository.Save(label);
            var returnValue = label;
            return Json(new { Result = "OK", Record = returnValue });
        }

        [HttpGet]
        public JsonResult Get(int? schemeId, string languageCode)
        {
            var items = new List<SiteLabelsViewModel>();

            if (!schemeId.HasValue)
            {
                schemeId = 1;
            }

            var labels = _siteLabelsRepository.All().Where(l => l.LanguageCode == languageCode && l.SchemeId == schemeId.GetValueOrDefault()).ToList();

            if (labels == null)
            {
                labels = _siteLabelsRepository.All().Where(l => l.LanguageCode == languageCode && l.SchemeId == 1).ToList();
            }
            
            return Json(new { Labels = labels }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetOptions(string languageCode)
        {
            var items = new List<SiteFieldValuesViewModel>();

            var options = _siteLabelsRepository.GetFieldValues().Where(l => l.LanguageCode == languageCode).Where(s => s.FieldType == "option").ToList();

            return Json(new { Options = options }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetRadioOptions(string languageCode)
        {
            var items = new List<SiteFieldValuesViewModel>();

            var options = _siteLabelsRepository.GetFieldValues().Where(l => l.LanguageCode == languageCode).Where(s => s.FieldType == "radio").ToList();

            return Json(new { Options = options }, JsonRequestBehavior.AllowGet);
        }

    }
}
