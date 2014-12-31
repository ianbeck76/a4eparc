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

namespace A4EPARC.Controllers
{
    public class SurveyController : BaseController
    {    
        private readonly IClientRepository _clientRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ISiteTextRepository _siteTextRepository;
        private readonly ICompanyRepository _companyRepository;

        public SurveyController(IClientRepository clientRepository, 
            IQuestionRepository questionRepository, 
            ISiteTextRepository siteTextRepository,
            ICompanyRepository companyRepository)
        {
            _clientRepository = clientRepository;
            _questionRepository = questionRepository;
            _siteTextRepository = siteTextRepository;
            _companyRepository = companyRepository;
        }

        [HttpGet]
        public ActionResult PageOne(int? id)
        {
            var model = id.HasValue ? _clientRepository.GetClient(id.Value) : new ClientViewModel();

            var schemes = _companyRepository.GetSchemes().Where(s => s.CompanyId == GetCurrentUser().CompanyId);
                                
            if(schemes.Count() == 1)
            {
                model.SchemeId = schemes.First().SchemeId;
            }
            if (schemes.Count() > 1)
            {
                model.SchemeDropdownList = GetSchemeDropdownList(schemes);
            }

            if (model.DateOfBirth.HasValue)
            {
                model.DateOfBirthDay = model.DateOfBirth.Value.Day;
                model.DateOfBirthMonth = model.DateOfBirth.Value.Month;
                model.DateOfBirthYear = model.DateOfBirth.Value.Year;
            }

            model = InitializeDropdowns(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult PageOne(ClientViewModel viewmodel)
        {
            var isDateOfBirthValid = CheckDateOfBirth(viewmodel);

            var schemes = _companyRepository.GetSchemes().Where(s => s.CompanyId == GetCurrentUser().CompanyId);

            if (viewmodel.SchemeId == 0)
            {
                if (schemes.Count() == 1)
                {
                    viewmodel.SchemeId = schemes.First().SchemeId;
                }
            }
            if (schemes.Count() > 1)
            {
                viewmodel.SchemeDropdownList = GetSchemeDropdownList(schemes);
            }

            if (!ModelState.IsValid || !isDateOfBirthValid || viewmodel.Gender == null)
            {
                viewmodel = InitializeDropdowns(viewmodel);
                return View(viewmodel);
            }

            viewmodel.DateOfBirth = new DateTime(
               viewmodel.DateOfBirthYear,
               viewmodel.DateOfBirthMonth,
               viewmodel.DateOfBirthDay);

            viewmodel.UserId = GetCurrentUser().Id;

            viewmodel.CreatedDate = DateTime.Now;
            viewmodel.FirstName = string.IsNullOrEmpty(viewmodel.FirstName) ? "N/A" : viewmodel.FirstName;
            viewmodel.Surname = string.IsNullOrEmpty(viewmodel.Surname) ? "N/A" : viewmodel.Surname;
            viewmodel.Deleted = false;

            var id = _clientRepository.InsertPerson(viewmodel);

            return RedirectToAction("PageTwo", new { id });
        }

        [HttpGet]
        public ActionResult PageTwo(int id)
        {
            var viewmodel = _clientRepository.GetClient(id);

            viewmodel.AimStatement = _siteTextRepository.Get(SiteTextCode.AimStatement, viewmodel.SchemeId, "en-GB");

            viewmodel.Questions = _questionRepository.Get(viewmodel.SchemeId, "en-GB");

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult PageTwo(ClientViewModel viewmodel, FormCollection formCollection)
        {
            #region Remove Not Needed ModelState Properties
            ModelState.Remove("JobSeekerID");
            ModelState.Remove("Agency");
            ModelState.Remove("CaseWorkerName");
            ModelState.Remove("CaseWorkerId");
            ModelState.Remove("CaseId");
            ModelState.Remove("FirstName");
            ModelState.Remove("Surname");
            ModelState.Remove("Gender");
            ModelState.Remove("LengthOfUnemployment");
            #endregion

            if (!viewmodel.AcceptCheckBox)
            {
                ModelState.AddModelError("CheckBox", "Required");
            }

            viewmodel.Questions = _questionRepository.Get(viewmodel.SchemeId, "en-GB");

            viewmodel.AimStatement = _siteTextRepository.Get(SiteTextCode.AimStatement, viewmodel.SchemeId, "en-GB");

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

            viewmodel.Result = CalculateDecision(viewmodel, sb.ToString().TrimEnd(','));

            if (viewmodel.Result.ActionIdToDisplay > ActionType.Undefined)
            {
                _clientRepository.InsertResult(viewmodel.Result);

                return RedirectToAction("PageThree", new {viewmodel.Id});
            }

            ModelState.AddModelError("WebserviceUnavailable", "Web service is currently unavailable - please try again");
            return View(viewmodel);
        }

        [HttpGet]
        public ActionResult PageThree(int id)
        {
            var viewmodel = _clientRepository.GetClient(id);

            if (viewmodel != null)
            {
                if (viewmodel.ActionIdToDisplay != (int)ActionType.Undefined)
                {
                    var siteText = _siteTextRepository.Get((SiteTextCode)viewmodel.ActionIdToDisplay, viewmodel.SchemeId, "en-GB").FirstOrDefault();
                    if (siteText != null)
                    {
                        viewmodel.ActionTypeName = siteText.Name;
                        viewmodel.ActionTypeDescription = siteText.Description;
                        viewmodel.ActionTypeSummary = siteText.Summary;
                    }
                }
            }

            return View(viewmodel);
        }

        private ResultViewModel CalculateDecision(ClientViewModel viewmodel, string answerString)
        {
            var actionScore = viewmodel.Questions.Where(r => r.ActionTypeId == 1).Select(r => r.Answer).Sum();
            var preContemplationScore = viewmodel.Questions.Where(r => r.ActionTypeId == 3).Select(r => r.Answer).Sum();
            var contemplationScore = viewmodel.Questions.Where(r => r.ActionTypeId == 2).Select(r => r.Answer).Sum();

            //Convert to total scores
            var ta = 50 + 10 * (actionScore - 13.2000) / 4.7460;
            var tp = 50 + 10 * (preContemplationScore - 7.4258) / 2.8306;
            var tc = 50 + 10 * (contemplationScore - 16.5484) / 2.5538;

            var reldist2 = Math.Pow((65.0558 - tp.GetValueOrDefault()), 2) + Math.Pow((41.7343 - tc.GetValueOrDefault()), 2) + Math.Pow((42.7966 - ta.GetValueOrDefault()), 2);
            var nradist2 = Math.Pow((60.0000 - tp.GetValueOrDefault()), 2) + Math.Pow((40.0000 - tc.GetValueOrDefault()), 2) + Math.Pow((60.0000 - ta.GetValueOrDefault()), 2);
            var refdist2 = Math.Pow((47.1980 - tp.GetValueOrDefault()), 2) + Math.Pow((50.1701 - tc.GetValueOrDefault()), 2) + Math.Pow((41.6665 - ta.GetValueOrDefault()), 2);
            var pardist2 = Math.Pow((45.3448 - tp.GetValueOrDefault()), 2) + Math.Pow((53.4616 - tc.GetValueOrDefault()), 2) + Math.Pow((58.6332 - ta.GetValueOrDefault()), 2);

            var distmin2a = Math.Min(reldist2, nradist2);
            var distmin2b = Math.Min(refdist2, pardist2);
            var distmin2 = Math.Min(distmin2a, distmin2b);

            var result = new ResultViewModel();
            result.ClientId = viewmodel.Id;
            result.ActionScore = actionScore.GetValueOrDefault();
            result.PreContemplationScore = preContemplationScore.GetValueOrDefault();
            result.ContemplationScore = contemplationScore.GetValueOrDefault();
            result.ActionScoreMatrix = (int)ta;
            result.PreContemplationScoreMatrix = (int)tp;
            result.ContemplationScoreMatrix = (int)tc;
            result.AnswerString = answerString;

            if (distmin2 == reldist2)
            {
                //stage 1 reluctant - precontemplation
                result.ActionIdToDisplay = ActionType.PreContemplation;
            }
            if (distmin2 == nradist2)
            {
                // stage 2 superficial action - Unauthentic Action
                result.ActionIdToDisplay = ActionType.UnauthenticAction;
            }
            if (distmin2 == refdist2)
            {
                // stage 3  Reflective = Contemplation
                result.ActionIdToDisplay = ActionType.Contemplation;
            }
            if (distmin2 == pardist2)
            {
                // stage 4 - if matrix score is greater than 44 Action else Preparation
                result.ActionIdToDisplay = result.ActionScoreMatrix > 55 ? ActionType.Action : ActionType.Preparation;
            }

            return result;
        }

        //Use this and remove above method if given to clients...
        private ResultViewModel GetDecision(ClientViewModel viewmodel, string answerString)
        {
            var url = string.Format("http://psychass.com/SOCActionService.svc/Get?key={0}&answers={1}", WebConfigurationManager.AppSettings["WebServiceKey"], answerString);

            var request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                var response = request.GetResponse();
                using(var responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream, Encoding.UTF8);
                    var stream =  reader.ReadToEnd();
                    var jsonObject = JObject.Parse(stream);

                    var jsonResult = JsonConvert.DeserializeObject<JsonResultModel>( jsonObject.ToString() );

                    var result = new ResultViewModel
                                     {
                                         ClientId = viewmodel.Id,
                                         ActionScore =
                                             viewmodel.Questions.Where(r => r.ActionTypeId == 1)
                                                      .Select(r => r.Answer.GetValueOrDefault())
                                                      .Sum(),
                                         PreContemplationScore =
                                             viewmodel.Questions.Where(r => r.ActionTypeId == 3)
                                                      .Select(r => r.Answer.GetValueOrDefault())
                                                      .Sum(),
                                         ContemplationScore =
                                             viewmodel.Questions.Where(r => r.ActionTypeId == 2)
                                                      .Select(r => r.Answer.GetValueOrDefault())
                                                      .Sum(),
                                         ActionScoreMatrix = Convert.ToInt32(jsonResult.GetResult.ActionScore),
                                         PreContemplationScoreMatrix = Convert.ToInt32(jsonResult.GetResult.PreContemplationScore),
                                         ContemplationScoreMatrix = Convert.ToInt32(jsonResult.GetResult.ContemplationScore),
                                         AnswerString = answerString
                                     };
                    switch (jsonResult.GetResult.Result)
                    {
                        case "PreContemplation":
                            {
                                result.ActionIdToDisplay =  ActionType.PreContemplation;
                                break;
                            }
                        case "UnreflectiveAction":
                            {
                                result.ActionIdToDisplay = ActionType.UnauthenticAction;
                                break;
                            }
                        case "Contemplation":
                            {
                                result.ActionIdToDisplay =  ActionType.Contemplation;
                                break;
                            }
                        case "Preparation":
                            {
                                result.ActionIdToDisplay = ActionType.Preparation;
                                break;
                            }
                        case "Action":
                            {
                                result.ActionIdToDisplay = ActionType.Action;
                                break;
                            }
                    }
                    return result;
                }
            }
            catch (WebException ex)
            {
                var errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    var errorText = reader.ReadToEnd();
                    // log errorText
                }
            }

            //action = 1, contemplation = 2, precontemplation = 3, preparer = 4, unauthenticaction = 5
 
            return new ResultViewModel{ ActionIdToDisplay = ActionType.Undefined };
        }

        private bool CheckDateOfBirth(ClientViewModel model)
        {
            try
            {
                var date = new DateTime(
                model.DateOfBirthYear,
                model.DateOfBirthMonth,
                model.DateOfBirthDay);
                return true;
            }
            catch (Exception e)
            {
                ModelState.AddModelError("InvalidDate", "Invalid DOB");
                return false;
            }
        }

        private List<string> GetLengthOfUnemploymentDropdownList()
        {
            var ddl = new List<string>();
            ddl.Add("0-2 months");
            ddl.Add("3-5 months");
            ddl.Add("6-12 months");
            ddl.Add("1-3 years");
            ddl.Add("4+ years");
            ddl.Add("N/A - currently employed");
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

        private ClientViewModel InitializeDropdowns(ClientViewModel model)
        {
            model.DayDropdownList = GetDayDropdownList();
            model.MonthDropdownList = GetMonthDropdownList();
            model.YearDropdownList = GetYearDropdownList();
            model.LengthOfUnemploymentDropdownList = GetLengthOfUnemploymentDropdownList();
            return model;
        }

    }
}
