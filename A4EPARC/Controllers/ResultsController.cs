using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
namespace A4EPARC.Controllers
{
    [Authorize(Roles = "IsViewer")]
    public class ResultsController : BaseController
    {

        private readonly IClientRepository _clientRepository;
        private readonly ISiteLabelsRepository _siteLabelsRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IQuestionRepository _questionRepository;

        public ResultsController(IClientRepository clientRepository,
             ISiteLabelsRepository siteLabelsRepository, 
             ICompanyRepository companyRepository, 
             IQuestionRepository questionRepository)
        {
            _clientRepository = clientRepository;
            _siteLabelsRepository = siteLabelsRepository;
            _companyRepository = companyRepository;
            _questionRepository = questionRepository;
        }

        public ActionResult Index(string datefrom, string dateto, string jobseekerId, 
                                string surname, string username, string company,
                                  GridSortOptions gridSortOptions, int? page)
        {

            ViewBag.EmptyBrowseSearchLink = GetEmptyBrowseResultsLink();

            if (String.IsNullOrWhiteSpace(gridSortOptions.Column))
            {
                gridSortOptions.Column = "CreatedDate";
                gridSortOptions.Direction = SortDirection.Descending;
            }

            if (datefrom == "null")
                datefrom = "";

            if (dateto == "null")
                dateto = "";
 
            if (jobseekerId == "null")
                jobseekerId = "";
 
            if (surname == "null")
                surname = "";

            if (username == "null")
                username = "";

            if (company == "null")
                company = "";

            if (!GetCurrentUser().IsSuperAdmin)
            {
                company = GetCompanyDetails().Name;
            }

            var query = _clientRepository.All(ConvertMinDate(datefrom), ConvertMaxDate(dateto), jobseekerId, surname, username, company);
            
            var model = new ClientResultListViewModel
            {
                GridSortOptions = gridSortOptions,
                Results = query.AsPagination(page.GetValueOrDefault() == 0 ? 1 : page.GetValueOrDefault(), 10),
                DateFrom = datefrom,
                DateTo = dateto,
                JobSeekerID = jobseekerId,
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

        public ActionResult GeneratePdf(int id)
        {
            var viewmodel = _clientRepository.GetClient(id);

            return new ActionAsPdf(
                           "Pdf",
                           new { id = id }) { FileName = viewmodel.Company + " SurveyDetails/" + id + ".pdf" };
        }

        public ActionResult GenerateGlsPdf(int id)
        {
            var viewmodel = _clientRepository.GetClient(id);

            switch (viewmodel.ActionIdToDisplay)
            {
                case 2:
                    return new ActionAsPdf(
                    "GlsSilverPdf",
                        new { id = id }) { FileName = viewmodel.Company + " Silver/" + id + ".pdf" };
                case 4:
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
            if (datefrom == "null")
                datefrom = "";

            if (dateto == "null")
                dateto = "";

            if (jobseekerid == "null")
                jobseekerid = "";

            if (surname == "null")
                surname = "";

            if (username == "null")
                username = "";

            if (company == "null")
                company = "";

            var data = _clientRepository.GetCsvData(ConvertMinDate(datefrom), ConvertMaxDate(dateto), jobseekerid, surname, username, company).ToList();

            var sb = new StringBuilder();

            if (AuthenticationService.GetCompanyId() == 11)
            {
                sb = GetGlsExcel(data);
            }
            else {
                sb = GetStandardExcel(data);
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
                    return "SILVER";
                case "action":
                case "preparation":
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
            }

            return viewmodel;
        }

        private StringBuilder GetGlsExcel(IList<ClientCsvModel> data)
        {
            var sb = new StringBuilder();
            sb.Append("CreatedDate,Username,FirstName,Surname,Previous Surveys,Date Of Birth,State,Action Name,Action Band,Answer String,Action Points,Contemplation Points,PreContemplation Points,Matrix Action Points,Matrix Contemplation Points,Matrix PreContemplation Points");
            sb.AppendLine(Environment.NewLine);

            foreach (var d in data)
            {
                sb.Append(d.CreatedDate + ",");
                sb.Append(d.Username + ",");
                sb.Append(d.FirstName + ",");
                sb.Append(d.Surname+ ",");
                sb.Append(d.HowManyTimesHasSurveyBeenCompleted + ",");
                sb.Append(d.DateOfBirth.ToString("dd/MM/yyyy ") + ",");
                sb.Append(d.State + ",");
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

        private StringBuilder GetStandardExcel(IList<ClientCsvModel> data) 
        {
            var sb = new StringBuilder();
            sb.Append("CreatedDate,Username,FirstName,Surname,CaseWorker Name,Case Worker ID,JobSeekerID,Previous Surveys,Date Of Birth,Gender,Length Of Unemployment,State,Stream,Action Name,Answer String,Action Points,Contemplation Points,PreContemplation Points,Matrix Action Points,Matrix Contemplation Points,Matrix PreContemplation Points,Comments");
            sb.AppendLine(Environment.NewLine);

            foreach (var d in data)
            {
                sb.Append(d.CreatedDate + ",");
                sb.Append(d.Username + ",");
                sb.Append(d.FirstName + ",");
                sb.Append(d.Surname + ",");
                sb.Append(d.CaseWorkerName + ",");
                sb.Append(d.CaseWorkerId + ",");
                sb.Append(d.JobSeekerID + ",");
                sb.Append(d.HowManyTimesHasSurveyBeenCompleted + ",");
                sb.Append(d.DateOfBirth.ToString("MM/dd/yyyy ") + ",");
                sb.Append(d.Gender + ",");
                sb.Append(d.LengthOfUnemployment + ",");
                sb.Append(d.State + ",");
                sb.Append(d.Stream + ",");
                sb.Append(d.ActionName + ",");
                sb.Append(d.AnswerString.Replace(',', '-') + ",");
                sb.Append(d.ActionPoints + ",");
                sb.Append(d.ContemplationPoints + ",");
                sb.Append(d.PreContemplationPoints + ",");
                sb.Append(d.MatrixActionPoints + ",");
                sb.Append(d.MatrixContemplationPoints + ",");
                sb.Append(d.MatrixPreContemplationPoints + ",");
                sb.Append(d.Comments);
                sb.Append(Environment.NewLine);
            }
            return sb;
        }

        private static string ObjectToCsvData<T>(List<T> objects)
        {
            var sb = new StringBuilder();
            Type t = typeof(T);
            PropertyInfo[] pi = t.GetProperties();
            for (int index = 0; index < pi.Length; index++)
            {
                var value = pi[index].Name;
                sb.Append(value.ToString().Contains(",") ? @"""" + value.ToString() + @"""" : value);
                if (index < pi.Length - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append(Environment.NewLine);

            foreach (var obj in objects)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException("obj", "Value can not be null or Nothing!");
                }

                for (int index = 0; index < pi.Length; index++)
                {
                    //sb.Append(@"=""" + pi[index].GetValue(obj, null)+ @"""");
                    var value = pi[index].GetValue(obj, null);

                    sb.Append(value != null && value.ToString().Contains(",") ? @"""" + value.ToString() + @"""" : value);

                    if (index < pi.Length - 1)
                    {
                        sb.Append(",");
                    }
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
