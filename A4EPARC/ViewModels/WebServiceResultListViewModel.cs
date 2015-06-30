using System.ComponentModel.DataAnnotations;
using MvcContrib.Pagination;
using MvcContrib.UI.Grid;
using System.Collections.Generic;

namespace A4EPARC.ViewModels
{
    public class WebServiceResultListViewModel
    {
        public IPagination<WebServiceResultsViewModel> Results { get; set; }
        public GridSortOptions GridSortOptions { get; set; }
        [Display(Name = "Date From")]
        public string DateFrom { get; set; }
        [Display(Name = "Date To")]
        public string DateTo { get; set; }
        public string JobSeekerID { get; set; }
        public string Company { get; set; }
        public List<string> Companies { get; set; }
        public string Environment { get; set; }
        public List<string> EnvironmentList { get; set; }
    }
}