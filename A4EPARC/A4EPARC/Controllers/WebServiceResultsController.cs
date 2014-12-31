using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using A4EPARC.Repositories;
using A4EPARC.Services;
using A4EPARC.ViewModels;
using MvcContrib.UI.Grid;

namespace A4EPARC.Controllers
{
    public class WebServiceResultsController : BaseController
    {
        private IWebServiceResultsRepository webServiceResultsRepository { get; set; }
        private IAuthenticationService authenticationService { get; set; }

        public WebServiceResultsController(IWebServiceResultsRepository _webServiceResultsRepository, 
            IAuthenticationService _authenticationService)
        {
            webServiceResultsRepository = _webServiceResultsRepository;
            authenticationService = _authenticationService;
        }

        [HttpGet]
        public ActionResult Index(GridSortOptions gridSortOptions, int? page, DateTime? fromDate, DateTime? toDate, string company, string jobSeekerID, string environmentType)
        {
            IQueryable<WebServiceResultsViewModel> query;
            
            bool showAll = false;

            query = webServiceResultsRepository.GetResultsView(showAll);           

            int? currentPage = page != null ? page : 1;
            int? nextPage = null;
            int? lastPage = null;
            int? firstPage = null;
            int? previousPage = null;
            int count = query.Count();
            const int pageSize = 20;

            company = string.IsNullOrWhiteSpace(company) ? null : company.ToLower();
            jobSeekerID = string.IsNullOrWhiteSpace(jobSeekerID) ? null : jobSeekerID.ToLower();
            environmentType = string.IsNullOrWhiteSpace(environmentType) || environmentType == "BOTH" ? null : environmentType.ToLower();
            var createdDateMin = fromDate;
            var createdDateMax = toDate.HasValue ? toDate.Value.AddDays(1).AddSeconds(-1) : (DateTime?)null;

            if (currentPage != null)
            {
                if (count > (pageSize * currentPage))
                {
                    nextPage = currentPage + 1;
                }

                if ((currentPage * pageSize) < count)
                {
                    lastPage = (int)Math.Ceiling((double)count / (double)pageSize);
                }

                if (currentPage != null && currentPage != 1)
                {
                    previousPage = currentPage - 1;
                    firstPage = 1;
                }
            }

            var pagedViewModel = new PagedViewModel<WebServiceResultsViewModel>
            {
                ViewData = ViewData,
                Query = query,
                GridSortOptions = gridSortOptions,
                DefaultSortColumn = "CreatedDate",
                CurrentPage = page,
                FirstPage = firstPage,
                PreviousPage = previousPage,
                LastPage = lastPage,
                NextPage = nextPage,
                PageSize = pageSize,
                ShowAllRecords = showAll,
                EnvironemntTypeList = new List<string> { "LIVE", "TEST", "BOTH"}
            }
            .AddFilter("fromDate", fromDate, c => c.CreatedDate > createdDateMin)
            .AddFilter("toDate", toDate, c => c.CreatedDate < createdDateMax)
            .AddFilter("company", company, c => c.CompanyName.ToLower() == company)
            .AddFilter("environmentType", environmentType, c => c.Environment.ToLower() == environmentType)
            .AddFilter("jobSeekerID", jobSeekerID, c => c.JobSeekerId.ToLower() == jobSeekerID)
            .Setup();

            return View(pagedViewModel);
        }

        [HttpPost]
        public ActionResult Index(DateTime? toDate, DateTime? fromDate, string environmentType, string jobSeekerID, GridSortOptions gridSortOptions, int? page, PagedViewModel<WebServiceResultsViewModel> model, string submitbutton)
        {
            if (submitbutton == "Print Selection")
            {
                var sb = new StringBuilder();

                var resultList = webServiceResultsRepository.GetCsvData(toDate.Value.AddDays(1).AddSeconds(-1), fromDate.Value, environmentType, jobSeekerID, model.ShowAllRecords);

                sb.Append("Company,CreatedDate,Environment,JobSeekerID,Action Type, Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9, Q10, Q11, Q12" + Environment.NewLine);

                foreach (var result in resultList)
                {
                    sb.Append(result.CompanyName + ",");
                    sb.Append(result.CreatedDate + ",");
                    sb.Append(result.EnvironmentType + ",");
                    sb.Append(result.JobSeekerID + ",");
                    sb.Append(result.ActionToDisplay + ",");
                    sb.Append(result.QuestionOne + ",");
                    sb.Append(result.QuestionTwo + ",");
                    sb.Append(result.QuestionThree + ",");
                    sb.Append(result.QuestionFour + ",");
                    sb.Append(result.QuestionFive + ",");
                    sb.Append(result.QuestionSix + ",");
                    sb.Append(result.QuestionSeven + ",");
                    sb.Append(result.QuestionEight + ",");
                    sb.Append(result.QuestionNine + ",");
                    sb.Append(result.QuestionTen + ",");
                    sb.Append(result.QuestionEleven + ",");
                    sb.Append(result.QuestionTwelve + ",");
                    sb.Append(Environment.NewLine);
                }

                return this.File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", string.Format("WebServiceResults-{0}.csv", DateTime.Now.ToString("g").Replace("/", "-").Replace(":", "_").Replace(" ", "-")));
            }

            IQueryable<WebServiceResultsViewModel> query;

            query = webServiceResultsRepository.GetResultsView(model.ShowAllRecords);

            int? currentPage = page != null ? page : 1;
            int? nextPage = null;
            int? lastPage = null;
            int? firstPage = null;
            int? previousPage = null;
            int count = query.Count();
            const int pageSize = 20;

            environmentType = string.IsNullOrWhiteSpace(environmentType) || environmentType == "BOTH" ? null : environmentType.ToLower();
            jobSeekerID = string.IsNullOrWhiteSpace(jobSeekerID) ? null : jobSeekerID.ToLower();
            var createdDateMin = fromDate;
            var createdDateMax = toDate;

            if (currentPage != null)
            {
                if (count > (pageSize * currentPage))
                {
                    nextPage = currentPage + 1;
                }

                if((currentPage * pageSize) < count)
                {
                    lastPage = (int)Math.Ceiling((double)count / (double)pageSize);
                }

                if (currentPage != null && currentPage != 1)
                {
                    previousPage = currentPage - 1;
                    firstPage = 1;
                }
            }
            
            var pagedViewModel = new PagedViewModel<WebServiceResultsViewModel>
            {
                ViewData = ViewData,
                Query = query,
                GridSortOptions = gridSortOptions,
                DefaultSortColumn = "CreatedDate",
                CurrentPage = page,
                FirstPage = firstPage,
                PreviousPage = previousPage,
                LastPage = lastPage,
                NextPage = nextPage,
                PageSize = pageSize,
                ShowAllRecords = model.ShowAllRecords,
                EnvironemntTypeList = new List<string> { "LIVE", "TEST", "BOTH" }
            }
            .AddFilter("fromDate", fromDate, c => c.CreatedDate > createdDateMin)
            .AddFilter("toDate", toDate, c => c.CreatedDate < createdDateMax)
            .AddFilter("environmentType", environmentType, c => c.Environment.ToLower() == environmentType)
            .AddFilter("jobSeekerID", jobSeekerID, c => c.JobSeekerId.ToLower() == jobSeekerID)
            .Setup();

            return View(pagedViewModel);
        }
    }
}

