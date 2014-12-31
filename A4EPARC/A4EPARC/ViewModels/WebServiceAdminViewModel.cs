using System;
using System.ComponentModel.DataAnnotations;

namespace A4EPARC.ViewModels
{
    public class WebServiceAdminViewModel
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        public string CompanyName { get; set; }

        public string Environment { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }

        public string JobSeekerId { get; set; }

        public string ActionResult { get; set; }

        public string AnswerList { get; set; }

    }
}