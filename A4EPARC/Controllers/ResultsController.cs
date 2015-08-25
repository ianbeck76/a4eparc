using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using A4EPARC.Enums;
using A4EPARC.Repositories;
using A4EPARC.ViewModels;
using MvcContrib.Pagination;
using MvcContrib.Sorting;
using MvcContrib.UI.Grid;
using Rotativa;
using A4EPARC.Services;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;
using System.Configuration;
namespace A4EPARC.Controllers
{
    [Authorize(Roles = "IsViewer")]
    public class ResultsController : AuthBaseController
    {

        private readonly IClientRepository _clientRepository;
        private readonly ISiteLabelsRepository _siteLabelsRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IQuestionsRepository _questionRepository;

        public ResultsController(IClientRepository clientRepository,
             ISiteLabelsRepository siteLabelsRepository, 
             ICompanyRepository companyRepository, 
             IQuestionsRepository questionRepository)
        {
            _clientRepository = clientRepository;
            _siteLabelsRepository = siteLabelsRepository;
            _companyRepository = companyRepository;
            _questionRepository = questionRepository;
        }

        public ActionResult Index(string datefrom, string dateto, string jobseekerid, 
                                string surname, string username, string company,
                                  GridSortOptions gridSortOptions, int? page)
        {

            if (String.IsNullOrWhiteSpace(gridSortOptions.Column))
            {
                gridSortOptions.Column = "CreatedDate";
                gridSortOptions.Direction = SortDirection.Descending;
            }

            datefrom = GetParameterValue("res", "datefrom", datefrom);
            dateto = GetParameterValue("res", "dateto", dateto);
            jobseekerid = GetParameterValue("res", "jobseekerid", jobseekerid);
            surname = GetParameterValue("res", "surname", surname);
            company = GetParameterValue("res", "company", company);
            username = GetParameterValue("res", "username", username);

            if (!GetCurrentUser().IsSuperAdmin)
            {
                company = GetCompanyDetails().Name;
            }

            var query = _clientRepository.All(ConvertMinDate(datefrom), ConvertMaxDate(dateto), jobseekerid, surname, username, company);
            
            var model = new ClientResultListViewModel
            {
                GridSortOptions = gridSortOptions,
                Results = query.AsPagination(page.GetValueOrDefault() == 0 ? 1 : page.GetValueOrDefault(), 10),
                DateFrom = datefrom,
                DateTo = dateto,
                JobSeekerID = jobseekerid,
                Surname = surname,
                Username = username,
                Company = company,
                Companies = _companyRepository.All().Select(c => c.Name).ToList()
            };

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var viewmodel = _clientRepository.GetClient(id);

            if (viewmodel != null)
            {
                if (viewmodel.ActionIdToDisplay != (int)ActionType.Undefined)
                {
                    viewmodel.SiteLabels = _siteLabelsRepository.Get(viewmodel.SchemeId, LanguageCode);
                    if (viewmodel.SiteLabels.Any())
                    {
                        viewmodel = AppendActionDetailsToViewModel(viewmodel);
                    }
                }
            }

            return View(viewmodel);
        }

        public ActionResult DetailsGls(int id)
        {
            var viewmodel = _clientRepository.GetClient(id);

            if (viewmodel != null)
            {
                if (viewmodel.ActionIdToDisplay != (int)ActionType.Undefined)
                {
                    viewmodel.SiteLabels = _siteLabelsRepository.Get(4, "en-GB");
                    if (viewmodel.SiteLabels.Any())
                    {
                        viewmodel = AppendActionDetailsToViewModel(viewmodel);
                    }
                }
            }

            return View(viewmodel);
        }

        public ActionResult Pdf(int id)
        {
            var viewmodel = _clientRepository.GetClient(id);

            if (viewmodel != null)
            {
                if (viewmodel.ActionIdToDisplay != (int)ActionType.Undefined)
                {
                    viewmodel.SiteLabels = _siteLabelsRepository.Get(viewmodel.SchemeId, LanguageCode);
                    if (viewmodel.SiteLabels.Any())
                    {
                        viewmodel = AppendActionDetailsToViewModel(viewmodel);
                    }
                }
            }

            return View(viewmodel);
        }

        public ActionResult GlsBronzePdf(int id)
        {
            return View(GetViewModel(id));
        }

        public ActionResult GlsSilverPdf(int id)
        {
           return View(GetViewModel(id));
        }

        public ActionResult GlsGoldPdf(int id)
        {
            return View(GetViewModel(id));
        }

        public ActionResult PreContemplationPdf(int id)
        {
            return View(GetViewModel(id));
        }

        public ActionResult GeneratePdf(int id)
        {
            var viewmodel = _clientRepository.GetClient(id);

            switch (viewmodel.ActionIdToDisplay)
            {
                case 1:
                    return new ActionAsPdf(
                    "PreContemplationPdf",
                        new { id = id }) { FileName = viewmodel.Company + " PreContemplation/" + id + ".pdf" };
                case 2:
                    return new ActionAsPdf(
                    "UnauthenticActionPdf",
                        new { id = id }) { FileName = viewmodel.Company + " UnauthenticAction/" + id + ".pdf" };
                case 3:
                    return new ActionAsPdf(
                    "ContemplationPdf",
                        new { id = id }) { FileName = viewmodel.Company + " Contemplation/" + id + ".pdf" };
                case 4:
                    return new ActionAsPdf(
                    "PreparationPdf",
                        new { id = id }) { FileName = viewmodel.Company + " Preparation/" + id + ".pdf" };
                case 5:
                    return new ActionAsPdf(
                    "ActionPdf",
                        new { id = id }) { FileName = viewmodel.Company + " Action/" + id + ".pdf" };
                default:
                    return new ActionAsPdf(
                    "UnauthenticActionPdf",
                        new { id = id }) { FileName = viewmodel.Company + " UnauthenticAction/" + id + ".pdf" };
            }
        }

        public ActionResult GenerateGlsPdf(int id)
        {
            var viewmodel = _clientRepository.GetClient(id);

            switch (viewmodel.ActionIdToDisplay)
            {
                case 2:
                case 4:
                    return new ActionAsPdf(
                    "GlsSilverPdf",
                        new { id = id }) { FileName = viewmodel.Company + " Silver/" + id + ".pdf" };
                case 5:
                    return new ActionAsPdf(
                    "GlsGoldPdf",
                        new { id = id }) { FileName = viewmodel.Company + " Gold/" + id + ".pdf" };
                case 1:
                case 3:
                    return new ActionAsPdf(
                    "GlsBronzePdf",
                        new { id = id }) { FileName = viewmodel.Company + " Bronze/" + id + ".pdf" };
                default:
                    return new ActionAsPdf(
                    "GlsBronzePdf",
                        new { id = id }) { FileName = viewmodel.Company + " Bronze/" + id + ".pdf" };
            }
        }

        public static string RenderPartialToString(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult Delete(int id)
        {
            var viewmodel = _clientRepository.GetClient(id);

            return RedirectToAction("Index", new { id });
        }

        public FileResult ExportList(string datefrom, string dateto, string jobseekerid, string surname, string username, string company)
        {
            datefrom = GetParameterValue("res", "datefrom", datefrom);
            dateto = GetParameterValue("res", "dateto", dateto);
            jobseekerid = GetParameterValue("res", "jobseekerid", jobseekerid);
            surname = GetParameterValue("res", "surname", surname);
            company = GetParameterValue("res", "company", company);
            username = GetParameterValue("res", "username", username);

            var items = _companyRepository.GetPageItems().Where(c => c.CompanyId == AuthenticationService.GetCompanyId()).ToList();

            var sb = new StringBuilder();

            var i = 0;

            if (GetCurrentUser().IsSuperAdmin)
            {
                foreach (var item in items.OrderBy(s => s.Name))
                {
                    sb.Append(item.Name);
                    sb.Append(",");
                }
            }
            else 
            {
                foreach (var item in items.Where(s => s.IsDisplay == true).OrderBy(s => s.Name))
                {
                    sb.Append(item.Name);
                    sb.Append(",");
                }
            }

            var fieldstring = "";
            if (sb.Length > 0)
            {
                fieldstring = sb.ToString().TrimEnd(',');                
            }

            var data = _clientRepository.GetCsvData(ConvertMinDate(datefrom), ConvertMaxDate(dateto), jobseekerid, surname, username, company, fieldstring).ToList();


            var companyId = AuthenticationService.GetCompanyId();

            if (companyId == 11 || companyId == 13)
            {
                sb = GetGlsExcel(data);
            }
            else {
                sb = GetStandardExcel(data, sb.ToString());
            }

            return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", string.Format("Client_Data_{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmmss")));
        }

        private string GetActionBand(string actionname) 
        {
            switch (actionname.ToLower())
            {
                case "precontemplation":
                case "contemplation":
                    return "BRONZE";
                case "unreflectiveaction":
                case "preparation":
                    return "SILVER";
                case "action":
                    return "GOLD";
                default:
                    return actionname;
            }
        }


        private ClientViewModel GetViewModel(int id)
        {
            var viewmodel = _clientRepository.GetClient(id);

            if (viewmodel != null)
            {
                if (viewmodel.ActionIdToDisplay != (int)ActionType.Undefined)
                {
                    viewmodel.SiteLabels = _siteLabelsRepository.Get(viewmodel.SchemeId, "en-GB");
                    if (viewmodel.SiteLabels.Any())
                    {
                        viewmodel = AppendActionDetailsToViewModel(viewmodel);
                    }
                }

                viewmodel.Questions = _questionRepository.Get(viewmodel.SchemeId, LanguageCode);

                if (!viewmodel.Questions.Any())
                {
                    _questionRepository.Get(1, "en-GB");
                }
            }

            return viewmodel;
        }

        private StringBuilder GetGlsExcel(IList<ClientCsvModel> data)
        {
            var sb = new StringBuilder();
            sb.Append("CreatedDate,Username,FirstName,Surname,Previous Surveys,Date Of Birth,State,RTO,Action Name,Action Band,Answer String,Action Points,Contemplation Points,PreContemplation Points,Matrix Action Points,Matrix Contemplation Points,Matrix PreContemplation Points");
            sb.AppendLine(Environment.NewLine);

            foreach (var d in data)
            {
                sb.Append(d.CreatedDate + ",");
                sb.Append(d.Username + ",");
                sb.Append(d.FirstName + ",");
                sb.Append(d.Surname+ ",");
                sb.Append(d.HowManyTimesHasSurveyBeenCompleted + ",");
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["UKDateFormat"]))
                {
                    sb.Append(d.DateOfBirth.ToString("dd/MM/yyyy ") + ",");
                }
                else
                {
                    sb.Append(d.DateOfBirth.ToString("MM/dd/yyyy ") + ",");
                }
                sb.Append(d.State + ",");
                sb.Append(d.RTO + ",");
                sb.Append(d.ActionName + ",");
                sb.Append(GetActionBand(d.ActionName) + ","); 
                sb.Append(d.AnswerString.Replace(',', '-') + ",");
                sb.Append(d.ActionPoints + ",");
                sb.Append(d.ContemplationPoints + ",");
                sb.Append(d.PreContemplationPoints + ",");
                sb.Append(d.MatrixActionPoints + ",");
                sb.Append(d.MatrixContemplationPoints + ",");
                sb.Append(d.MatrixPreContemplationPoints);
                sb.Append(Environment.NewLine);
            }
            return sb;
        }

        private StringBuilder GetStandardExcel(IList<ClientCsvModel> data, string fieldstring) 
        {
            var sb = new StringBuilder();
            sb.Append("CreatedDate,Company,Username,Previous Surveys,Action Name,Answer String,Action Points,Contemplation Points,PreContemplation Points,Matrix Action Points,Matrix Contemplation Points,Matrix PreContemplation Points," + fieldstring);

            sb.AppendLine(Environment.NewLine);

            foreach (var d in data)
            {
                sb.Append(d.CreatedDate + ",");
                sb.Append(d.Company + ",");
                sb.Append(d.Username + ",");
                sb.Append(d.HowManyTimesHasSurveyBeenCompleted + ",");
                sb.Append(d.ActionName + ",");
                sb.Append(d.AnswerString.Replace(',', '-') + ",");
                sb.Append(d.ActionPoints + ",");
                sb.Append(d.ContemplationPoints + ",");
                sb.Append(d.PreContemplationPoints + ",");
                sb.Append(d.MatrixActionPoints + ",");
                sb.Append(d.MatrixContemplationPoints + ",");
                sb.Append(d.MatrixPreContemplationPoints + ",");
                if (fieldstring.Contains("Agency"))
                {
                    sb.Append(d.Agency + ",");
                }
                if (fieldstring.Contains("CaseId"))
                {
                    sb.Append(d.CaseId + ",");
                }
                if (fieldstring.Contains("CaseWorkerId"))
                {
                    sb.Append(d.CaseWorkerId + ",");
                }
                if (fieldstring.Contains("CaseWorkerName"))
                {
                    sb.Append(d.CaseWorkerName + ",");
                }
                if (fieldstring.Contains("Comments"))
                {
                    sb.Append(d.Comments + ",");
                }
                if (fieldstring.Contains("CompletedAllFiveWorkshops"))
                {
                    sb.Append(d.CompletedAllFiveWorkshops + ",");
                }
                if (fieldstring.Contains("DateOfBirth"))
                {
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["UKDateFormat"]))
                    {
                        sb.Append(d.DateOfBirth.ToString("dd/MM/yyyy ") + ",");
                    }
                    else
                    {
                        sb.Append(d.DateOfBirth.ToString("MM/dd/yyyy ") + ",");
                    }
                }
                if (fieldstring.Contains("EnrolmentID"))
                {
                    sb.Append(d.EnrolmentID + ",");
                }
                if(fieldstring.Contains("FirstName"))
                {
                    sb.Append(d.FirstName + ",");
                }
                if (fieldstring.Contains("Gender"))
                {
                    sb.Append(d.Gender + ",");
                }
                if (fieldstring.Contains("HasDiplomaOrGED"))
                {
                    sb.Append(d.HasDiplomaOrGED + ",");
                }
                if (fieldstring.Contains("IsCurrentlyCollectingBenefits"))
                {
                    sb.Append(d.IsCurrentlyCollectingBenefits + ",");
                }
                if (fieldstring.Contains("IsIslander"))
                {
                    sb.Append(d.IsIslander + ",");
                }
                if (fieldstring.Contains("IsOverEighteen"))
                {
                    sb.Append(d.IsOverEighteen + ",");
                }
                if (fieldstring.Contains("IsReassessment"))
                {
                    sb.Append(d.IsReassessment + ",");
                }
                if (fieldstring.Contains("JobSeekerID"))
                {
                    sb.Append(d.JobSeekerID + ",");
                }
                if (fieldstring.Contains("LengthOfUnemployment"))
                {
                    sb.Append(d.LengthOfUnemployment + ",");
                }
                if (fieldstring.Contains("Provider"))
                {
                    sb.Append(d.Provider + ",");
                }
                if (fieldstring.Contains("Project"))
                {
                    sb.Append(d.Project + ",");
                }
                if (fieldstring.Contains("RTO"))
                {
                    sb.Append(d.RTO + ",");
                }
                if (fieldstring.Contains("SchemeId"))
                {
                    sb.Append(d.SchemeName + ",");
                }
                if (fieldstring.Contains("State"))
                {
                    sb.Append(d.State + ",");
                }
                if (fieldstring.Contains("Stream"))
                {
                    sb.Append(d.Stream + ",");
                }
                if (fieldstring.Contains("Surname"))
                {
                    sb.Append(d.Surname + ",");
                }
                if (fieldstring.Contains("UnemploymentInsuranceId"))
                {
                    sb.Append(d.UnemploymentInsuranceId + ",");
                }
                sb.Append(Environment.NewLine);
            }
            return sb;
        }
    }
}
