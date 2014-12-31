using System;
using System.Linq;
using System.Web.Mvc;
using A4EPARC.Repositories;
using A4EPARC.Services;
using A4EPARC.ViewModels;

namespace A4EPARC.Controllers
{
    [Authorize(Roles = "IsAdmin")]
    public class QuestionAdminController : BaseController
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IUserRepository _userRepository;
        
        public QuestionAdminController(IQuestionRepository questionRepository, IUserRepository userRepository)
        {
            _questionRepository = questionRepository;
            _userRepository = userRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetRows(int jtStartIndex, int jtPageSize, string jtSorting)
        {
            try
            {
                var list = _questionRepository.GetJtableView();

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
