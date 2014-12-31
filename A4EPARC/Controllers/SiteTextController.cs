using System;
using System.Linq;
using System.Web.Mvc;
using A4EPARC.Repositories;
using A4EPARC.ViewModels;
using System.Collections.Generic;

namespace A4EPARC.Controllers
{
    [Authorize(Roles = "IsAdmin")]
    public class SiteTextController : Controller
    {
        private readonly ISiteTextRepository _siteTextRepository;
        
        public SiteTextController(ISiteTextRepository siteTextRepository)
        {
            _siteTextRepository = siteTextRepository;
        }

        #region User

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetRows(string code, int jtStartIndex, int jtPageSize, string jtSorting)
        {
            try
            {
                var list = _siteTextRepository.GetJtableView();

                if (!string.IsNullOrWhiteSpace(code))
                {
                    list = list.Where(s => s.Code.Equals(Convert.ToInt32(code))).ToList();
                }

                var models = list.Select(item => new SiteTextViewModel
                {
                    Id = item.Id,
                    Code = item.Code,
                    LanguageCode = item.LanguageCode,
                    SchemeId = item.SchemeId,
                    Description = item.Description,
                    Summary = item.Summary,
                    Name = item.Name,
                    OrderNumber = item.OrderNumber,
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
        public JsonResult AddRow(SiteTextViewModel model)
        {
            try
            {
                if (_siteTextRepository.SingleOrDefault("Code = @Code AND SchemeId = @SchemeId AND Name = @Name", new {model.Code, model.SchemeId, model.Name}) != null)
                    return Json(new {Result = "Error", Message = "Must be unique"});

                model.LanguageCode = "en-GB";
                    _siteTextRepository.Add(model);

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

        public JsonResult EditRow(SiteTextViewModel model)
        {
            var sitetext = _siteTextRepository.SingleOrDefault(model.Id);
            sitetext.OrderNumber = model.OrderNumber;
            sitetext.Name = model.Name;
            sitetext.Summary = model.Summary;
            sitetext.Description = model.Description;
            _siteTextRepository.Save(sitetext);
            var returnValue = sitetext;
            return Json(new { Result = "OK", Record = returnValue });
        }

        #endregion
    }
}
