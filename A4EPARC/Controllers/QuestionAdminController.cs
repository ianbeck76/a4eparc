using System;
using System.Linq;
using System.Web.Mvc;
using A4EPARC.Repositories;
using A4EPARC.Services;
using A4EPARC.ViewModels;

namespace A4EPARC.Controllers
{
    [Authorize(Roles = "IsSuperAdmin")]
    public class QuestionAdminController : AuthBaseController
    {
        private readonly IQuestionsRepository _questionRepository;
        private readonly IUserRepository _userRepository;
        
        public QuestionAdminController(IQuestionsRepository questionRepository, IUserRepository userRepository)
        {
            _questionRepository = questionRepository;
            _userRepository = userRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetRows(int? scheme, string language, int jtStartIndex, int jtPageSize, string jtSorting)
        {
            try
            {
                var list = _questionRepository.GetJtableView().Where(q => q.SchemeId == scheme.GetValueOrDefault() && q.LanguageCode == language).ToList();

                var models = list.Select(item => new QuestionViewModel
                {
                    Id = item.Id,
                    Code = item.Code,
                    LanguageCode = item.LanguageCode,
                    SchemeId = item.SchemeId,
                    Description = item.Description,
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

        public JsonResult EditRow(QuestionViewModel model)
        {
            _questionRepository.Save(model);
            var returnValue = _questionRepository.SingleOrDefault(model.Id);
            return Json(new { Result = "OK", Record = returnValue });
        }

    }
}
