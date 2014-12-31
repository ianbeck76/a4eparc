using System.ComponentModel.DataAnnotations;
using MvcContrib.Pagination;
using MvcContrib.UI.Grid;

namespace A4EPARC.ViewModels
{
    public class ClientResultListViewModel
    {
        public IPagination<ClientResultViewModel> Results { get; set; }
        public GridSortOptions GridSortOptions { get; set; }
        [Display(Name = "Date From")]
        public string DateFrom { get; set; }
        [Display(Name = "Date To")]
        public string DateTo { get; set; }
        public string CaseID { get; set; }
        public string ClientID { get; set; }
    }
}