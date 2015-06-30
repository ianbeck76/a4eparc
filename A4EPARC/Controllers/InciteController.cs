using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using A4EPARC.Enums;
using A4EPARC.Repositories;
using A4EPARC.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.XPath;
using A4EPARC.Services;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace A4EPARC.Controllers
{
    public class InciteController : BaseController
    {    
        private readonly IClientRepository _clientRepository;
        private readonly IQuestionsService _questionsService;
        private readonly ICompanyRepository _companyRepository;
        private readonly ISiteLabelsRepository _siteLabelsRepository;
        private readonly IResultService _resultService;
        
        public InciteController(IClientRepository clientRepository, 
            IQuestionsService questionsService, 
            ISiteLabelsRepository siteLabelsRepository,
            ICompanyRepository companyRepository,
            IResultService resultService)
        {
            _clientRepository = clientRepository;
            _questionsService = questionsService;
            _siteLabelsRepository = siteLabelsRepository;
            _companyRepository = companyRepository;
            _resultService = resultService;
        }

        [HttpGet]
        public ActionResult Index()
        {
           
            var viewmodel = new ClientViewModel();

            var companyId = 14;

            viewmodel = SetScheme(viewmodel, companyId);

            viewmodel.SiteLabels = _siteLabelsRepository.Get(viewmodel.SchemeId, LanguageCode);

            viewmodel.Questions = _questionsService.Get(viewmodel.SchemeId, LanguageCode);

            viewmodel.PageItems = GetPageItems(companyId).Where(c => c.IsDisplay.GetValueOrDefault() == true);

            viewmodel = InitializeDropdowns(viewmodel, companyId);

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Index(ClientViewModel viewmodel)
        {
            var companyId = 14;

            viewmodel = SetScheme(viewmodel, companyId);
            viewmodel.SchemeId = viewmodel.SchemeId > 0 ? viewmodel.SchemeId : 1;

            viewmodel.PageItems = GetPageItems(companyId).Where(c => c.IsDisplay.GetValueOrDefault() == true);

            viewmodel.SiteLabels = _siteLabelsRepository.Get(viewmodel.SchemeId, LanguageCode);

            if(!ModelState.IsValid)
            {
                viewmodel = InitializeDropdowns(viewmodel, companyId);
                return View(viewmodel);
            }

            viewmodel.UserId = Convert.ToInt32(ConfigurationManager.AppSettings["InciteUserId"]);

            viewmodel.CreatedDate = DateTime.Now;
            viewmodel.FirstName = string.IsNullOrEmpty(viewmodel.FirstName) ? "N/A" : viewmodel.FirstName;
            viewmodel.Surname = string.IsNullOrEmpty(viewmodel.Surname) ? "N/A" : viewmodel.Surname;
            viewmodel.Deleted = false;

            if (!string.IsNullOrWhiteSpace(viewmodel.JobSeekerID))
            {
                viewmodel.HowManyTimesHasSurveyBeenCompleted = _clientRepository.GetNumberOfPreviousAttempts(viewmodel.JobSeekerID);
            }

            var id = _clientRepository.InsertPerson(viewmodel);

            return RedirectToAction("PageTwo", new { id });
        }

        [HttpGet]
        public ActionResult PageTwo(int id)
        {
            var viewmodel = _clientRepository.GetClient(id);
         
            viewmodel.SiteLabels = _siteLabelsRepository.Get(viewmodel.SchemeId, LanguageCode);

            viewmodel.Questions = _questionsService.Get(viewmodel.SchemeId, LanguageCode);

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult PageTwo(ClientViewModel viewmodel, FormCollection formCollection)
        {     
            viewmodel.SiteLabels = _siteLabelsRepository.Get(viewmodel.SchemeId, LanguageCode);

            viewmodel.Questions = _questionsService.Get(viewmodel.SchemeId, LanguageCode);

            var sb = new StringBuilder();

            foreach (var question in viewmodel.Questions)
            {
                var answer = formCollection[question.Code];

                if (string.IsNullOrEmpty(answer))
                {
                    ModelState.AddModelError(question.Code, "Required");
                }
                else
                {
                    question.Answer = Convert.ToInt32(answer);
                    sb.Append(question.Answer + ","); 
                }
            }

            if (viewmodel.ResultId > 0)
            {
                ModelState.AddModelError("RecordAlreadyAdded", "This record has already been added - go back to home page to create a new record");
            }

            if (!ModelState.IsValid)
            {
                return View(viewmodel);
            }

            viewmodel.Result = _resultService.CalculateDecision(viewmodel, sb.ToString().TrimEnd(','));

            if (viewmodel.Result.ActionIdToDisplay > ActionType.Undefined)
            {
                _clientRepository.InsertResult(viewmodel.Result);

                return RedirectToAction("ThankYou", new {viewmodel.Id});                
            }

            return View(viewmodel);
        }

        [HttpGet]
        public ActionResult PageThree(int id)
        {
            var viewmodel = _clientRepository.GetClient(id);

            viewmodel.SiteLabels = _siteLabelsRepository.Get(viewmodel.SchemeId, LanguageCode);

            if (viewmodel != null)
            {
                viewmodel = AppendActionDetailsToViewModel(viewmodel);
            }

            return View(viewmodel);
        }

        [HttpGet]
        public ActionResult ThankYou(int id)
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetQuestionLabels(int? schemeId, string languageCode)
        {
            var labels = new List<string>();
            var questions = _questionsService.Get(schemeId.GetValueOrDefault(), languageCode);
            if (questions.Any())
            {
                labels = questions.Select(s => s.Description).ToList();
            }
            return Json(new { Labels = labels }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetLanguageOptions()
        {
            var languageoptions = new List<KeyValuePair<string, string>>();
            var options = _companyRepository.GetLanguages().Where(c => c.CompanyId == AuthenticationService.GetCompanyId());
            if (options.Any())
            {
                languageoptions = options.Select(s => new KeyValuePair<string, string>(s.Code, s.Description)).ToList();
            }
            else
            {
                languageoptions = new List<KeyValuePair<string, string>>{new KeyValuePair<string,string>("en-GB", "English")};
            }
            return Json(new { Options = languageoptions }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<CompanyPageItemViewModel> GetPageItems(int companyId) 
        {
            var items = new List<CompanyPageItemViewModel>();
            var pageitems = _companyRepository.GetPageItems().Where(c => c.CompanyId == companyId).ToList();
            if (!pageitems.Any())
            {
                pageitems = _companyRepository.GetPageItems().Where(c => c.CompanyId == 1).ToList();
            }
            return pageitems;
        }

        [HttpGet]
        public JsonResult GetPageItems(int? companyId) 
        {
            var pageitems = GetPageItems(companyId.GetValueOrDefault());
            return Json(new { PageItems = pageitems }, JsonRequestBehavior.AllowGet);
        }

        private string ConvertYesNoToTrueFalse(string yesno)
        {
            return string.IsNullOrEmpty(yesno) ? yesno == "Yes" ? "true" : "false": null;
        }

        private bool CheckDateOfBirth(ClientViewModel model)
        {
            try
            {
                var date = new DateTime(
                model.DateOfBirthYear,
                model.DateOfBirthMonth,
                model.DateOfBirthDay);
                model.DateOfBirth = date;
                return true;
            }
            catch (Exception e)
            {
                ModelState.AddModelError("InvalidDate", "Invalid");
                return false;
            }
        }

        private List<string> GetCompanySelectValues(int companyId, int key)
        {
            var values = _companyRepository.GetSelectValues().Where(c => c.CompanyId == companyId && c.Key == key);

            if (values.Any())
            {
                return values.Select(c => c.Value).ToList();
            }
            return new List<string>();
        }

        private List<string> GetStreamDropdownList(int companyId)
        {
            var values = _companyRepository.GetSelectValues().Where(c => c.CompanyId == companyId && c.Key == (int)SelectKey.Stream);

            if (values.Any())
            {
                return values.Select(c => c.Value).ToList();
            }

            var ddl = new List<string>();
            ddl.Add("Stream 1");
            ddl.Add("Stream 2");
            ddl.Add("Stream 3");
            ddl.Add("Stream 4");
            ddl.Add("DES");
            return ddl;
        }

        private IEnumerable<KeyValuePair<int, string>> GetSchemeDropdownList(IEnumerable<CompanySchemeViewModel> schemes )
        {
            return schemes.Select(scheme => new KeyValuePair<int, string>(scheme.SchemeId, scheme.SchemeName)).ToList();
        }

        private List<int> GetDayDropdownList()
        {
            var days = new List<int>();
            for (int i = 1; i < 32; i++)
            {
                days.Add(i);
            }
            return days;
        }

        private List<KeyValuePair<int, string>> GetMonthDropdownList()
        {
            var months = new List<KeyValuePair<int, string>>();
            months.Add(new KeyValuePair<int, string>(1, "January"));
            months.Add(new KeyValuePair<int, string>(2, "February"));
            months.Add(new KeyValuePair<int, string>(3, "March"));
            months.Add(new KeyValuePair<int, string>(4, "April"));
            months.Add(new KeyValuePair<int, string>(5, "May"));
            months.Add(new KeyValuePair<int, string>(6, "June"));
            months.Add(new KeyValuePair<int, string>(7, "July"));
            months.Add(new KeyValuePair<int, string>(8, "August"));
            months.Add(new KeyValuePair<int, string>(9, "September"));
            months.Add(new KeyValuePair<int, string>(10, "October"));
            months.Add(new KeyValuePair<int, string>(11, "November"));
            months.Add(new KeyValuePair<int, string>(12, "December"));
            return months;
        }

        private List<int> GetYearDropdownList()
        {
            var years = new List<int>();
            for (int i = DateTime.UtcNow.Year - 15; i > 1940; i--)
            {
                years.Add(i);
            }
            return years;
        }

        private ClientViewModel SetScheme(ClientViewModel model, int companyId) 
        {
            var schemes = _companyRepository.GetSchemes().Where(s => s.CompanyId == companyId);

            if (model.SchemeId == 0)
            {
                if (schemes.Count() == 1)
                {
                    model.SchemeId = schemes.First().SchemeId;
                }
            }
            if (schemes.Count() > 1)
            {
                model.SchemeDropdownList = GetSchemeDropdownList(schemes);
            }

            return model;
        }

        private ClientViewModel InitializeDropdowns(ClientViewModel model, int companyId)
        {
            model.DayDropdownList = GetDayDropdownList();
            model.MonthDropdownList = GetMonthDropdownList();
            model.YearDropdownList = GetYearDropdownList();
            model.LengthOfUnemploymentDropdownList = GetCompanySelectValues(companyId, (int)SelectKey.LengthOfUnemployment);
            model.StateDropdownList = GetCompanySelectValues(companyId, (int)SelectKey.State);
            model.StreamDropdownList = GetStreamDropdownList(companyId);
            model.RTODropdownList = GetCompanySelectValues(companyId, (int)SelectKey.RTO);
            return model;
        }
    }
}
