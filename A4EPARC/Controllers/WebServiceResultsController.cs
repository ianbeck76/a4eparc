using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using A4EPARC.Repositories;
using A4EPARC.Services;
using A4EPARC.ViewModels;
using MvcContrib.Pagination;
using MvcContrib.Sorting;
using MvcContrib.UI.Grid;

namespace A4EPARC.Controllers
{
    public class WebServiceResultsController : AuthBaseController
    {
        private IWebServiceResultsRepository _webServiceResultsRepository { get; set; }
        private IAuthenticationService _authenticationService { get; set; }
        private ICompanyRepository _companyRepository { get; set; }

        public WebServiceResultsController(IWebServiceResultsRepository webServiceResultsRepository, 
            IAuthenticationService authenticationService, ICompanyRepository companyRepository)
        {
            _webServiceResultsRepository = webServiceResultsRepository;
            _authenticationService = authenticationService;
            _companyRepository = companyRepository;
        }

        public ActionResult Index(string datefrom, string dateto, string jobseekerid, string environment, string company, GridSortOptions gridSortOptions, int? page)
        {
            if (String.IsNullOrWhiteSpace(gridSortOptions.Column))
            {
                gridSortOptions.Column = "CreatedDate";
                gridSortOptions.Direction = SortDirection.Descending;
            }

            datefrom = GetParameterValue("wsr", "datefrom", datefrom);
            dateto = GetParameterValue("wsr", "dateto", dateto);
            jobseekerid = GetParameterValue("wsr", "jobseekerid", jobseekerid);
            environment = GetParameterValue("wsr", "environment", ConvertEnvironment(environment));
            company = GetParameterValue("wsr", "company", company);

            if (!GetCurrentUser().IsSuperAdmin)
            {
                company = GetCompanyDetails().Name;
            }

            var query = _webServiceResultsRepository.GetResultsView(ConvertMinDate(datefrom), ConvertMaxDate(dateto), jobseekerid, environment, company);

            var model = new WebServiceResultListViewModel
            {
                GridSortOptions = gridSortOptions,
                Results = query.AsPagination(page.GetValueOrDefault() == 0 ? 1 : page.GetValueOrDefault(), 10),
                DateFrom = datefrom,
                DateTo = dateto,
                JobSeekerID = jobseekerid,
                Company = company,
                Companies = _companyRepository.All().Where(c => c.IsActive == true).Select(c => c.Name).ToList(),
                EnvironmentList = new List<string> { "BOTH", "LIVE", "TEST" },
                Environment = environment
            };

            return View(model);
        }

        public FileResult ExportList(string datefrom, string dateto, string jobseekerid, string environment, string company)
        {
            datefrom = GetParameterValue("wsr", "datefrom", datefrom);
            dateto = GetParameterValue("wsr", "dateto", dateto);
            jobseekerid = GetParameterValue("wsr", "jobseekerid", jobseekerid);
            environment = GetParameterValue("wsr", "environment", ConvertEnvironment(environment));
            company = GetParameterValue("wsr", "company", company);

            if (!GetCurrentUser().IsSuperAdmin)
            {
                company = GetCompanyDetails().Name;
            }

            var query = _webServiceResultsRepository.GetResultsView(ConvertMinDate(datefrom), ConvertMaxDate(dateto), jobseekerid, environment, company);

            var sb = new StringBuilder();

            sb = GetWebServiceExcel(query.ToList());
       
            return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", string.Format("WebService_Data_{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmmss")));
        }

        private string ConvertEnvironment(string environment)
        {
            if (environment != null)
            {
                if (environment == "LIVE")
                {
                    environment = "PROD";
                }
                if (environment == "BOTH")
                {
                    environment = "";
                }
            }
            return environment;
        }

        private StringBuilder GetWebServiceExcel(IList<WebServiceResultsViewModel> data)
        {
            var sb = new StringBuilder();
            sb.Append("CreatedDate,ActionName,AnswerList,Environment,JobSeekerID,Company");
            sb.AppendLine(Environment.NewLine);

            foreach (var d in data)
            {
                sb.Append(d.CreatedDate + ",");
                sb.Append(d.ActionResult + ",");
                sb.Append(d.AnswerList.Replace(',', '-') + ",");
                sb.Append(d.Environment + ",");
                sb.Append(d.JobSeekerId + ",");
                sb.Append(d.Company);
                sb.Append(Environment.NewLine);
            }
            return sb;
        }

    }
}

