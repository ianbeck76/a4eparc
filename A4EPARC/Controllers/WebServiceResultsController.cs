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

        public ActionResult Index(string fromdate, string todate, string jobSeekerid, string environment, string company, GridSortOptions gridSortOptions, int? page)
        {
            int? currentPage = page != null ? page : 1;
            int? nextPage = null;
            int? lastPage = null;
            int? firstPage = null;
            int? previousPage = null;
            const int pageSize = 20;

            company = string.IsNullOrWhiteSpace(company) ? null : company.ToLower();
            jobSeekerid = string.IsNullOrWhiteSpace(jobSeekerid) ? null : jobSeekerid.ToLower();
            environment = string.IsNullOrWhiteSpace(environment) || environment == "BOTH" ? null : environment.ToLower();
         //   var createdDateMin = fromdate;
       //     var createdDateMax = todate.HasValue ? todate.Value.AddDays(1).AddSeconds(-1) : (DateTime?)null;

            IQueryable<WebServiceResultsViewModel> query;

            query = _webServiceResultsRepository.GetResultsView();           

            if (currentPage != null)
            {
                if (query.Count() > (pageSize * currentPage))
                {
                    nextPage = currentPage + 1;
                }

                if ((currentPage * pageSize) < query.Count())
                {
                    lastPage = (int)Math.Ceiling((double)query.Count() / (double)pageSize);
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
                Company = company,
                Companies = _companyRepository.All().Select(c => c.Name).ToList()
            }
//            .AddFilter("fromDate", fromdate, c => c.CreatedDate > createdDateMin)
  //          .AddFilter("toDate", todate, c => c.CreatedDate < createdDateMax)
            .AddFilter("company", company, c => c.CompanyName.ToLower() == company)
            .AddFilter("environmentType", environment, c => c.Environment.ToLower() == environment)
            .AddFilter("jobSeekerID", jobSeekerid, c => c.JobSeekerId.ToLower() == jobSeekerid)
            .Setup();

            return View(pagedViewModel);
        }

     
    }
}

