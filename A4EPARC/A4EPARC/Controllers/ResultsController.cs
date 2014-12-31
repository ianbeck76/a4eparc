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
namespace A4EPARC.Controllers
{
    [Authorize(Roles = "IsViewer")]
    public class ResultsController : BaseController
    {

        private readonly IClientRepository _clientRepository; 
        private readonly ISiteTextRepository _siteTextRepository;

        public ResultsController(IClientRepository clientRepository, ISiteTextRepository siteTextRepository)
        {
            _clientRepository = clientRepository;
            _siteTextRepository = siteTextRepository;
        }

        public ActionResult Index(string datefrom, string dateto, string caseId, string clientId,
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
 
            if (caseId == "null")
                caseId = "";
 
            if (clientId == "null")
                clientId = "";
 
            var query = _clientRepository.All(ConvertMinDate(datefrom), ConvertMaxDate(dateto), caseId, clientId);
            
            var model = new ClientResultListViewModel
            {
                GridSortOptions = gridSortOptions,
                Results = query.AsPagination(page.GetValueOrDefault() == 0 ? 1 : page.GetValueOrDefault(), 10),
                DateFrom = datefrom,
                DateTo = dateto,
                CaseID = caseId,
                ClientID = clientId
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

        public ActionResult Delete(int id)
        {
            var viewmodel = _clientRepository.GetClient(id);

            return RedirectToAction("Index", new { id });
        }

        public FileResult PrintList(string datefrom, string dateto, string caseId)
        {
            if (datefrom == "null")
                datefrom = "";

            if (dateto == "null")
                dateto = "";

            if (caseId == "null")
                caseId = "";

            var data = _clientRepository.GetCsvData(ConvertMinDate(datefrom), ConvertMaxDate(dateto), caseId).ToList();

            var sb = new StringBuilder();
            sb.Append("CreatedDate,Username,Office,CaseWorker Name,Case Worker ID,Case ID,Client ID,Date Of Birth,Gender,Length Of Unemployment,Action Name,Answer String,Action Points,Contemplation Points,PreContemplation Points,Matrix Action Points,Matrix Contemplation Points,Matrix PreContemplation Points,Comments");
            sb.AppendLine(Environment.NewLine);

            foreach (var d in data)
            {
                sb.Append(d.CreatedDate + ",");
                sb.Append(d.Username + ",");
                sb.Append(d.CaseWorkerName + ",");
                sb.Append(d.CaseWorkerId + ",");
                sb.Append(d.CaseId + ",");
                sb.Append(d.DateOfBirth.ToString("MM/dd/yyyy ") + ",");
                sb.Append(d.Gender + ",");
                sb.Append(d.LengthOfUnemployment + ",");
                sb.Append(d.ActionName + ",");
                sb.Append(d.AnswerString.Replace(',','-') + ",");
                sb.Append(d.ActionPoints + ",");
                sb.Append(d.ContemplationPoints + ",");
                sb.Append(d.PreContemplationPoints + ",");
                sb.Append(d.MatrixActionPoints + ",");
                sb.Append(d.MatrixContemplationPoints + ",");
                sb.Append(d.MatrixPreContemplationPoints + ",");
                sb.Append(d.Comments);
                sb.AppendLine(Environment.NewLine);
            }

            return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", string.Format("Client_Data_{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmmss")));
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
