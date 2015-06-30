using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace A4EPARC.ViewModels
{
    public class WebServiceResultsViewModel
    {
            [ScaffoldColumn(false)]
            public string Id { get; set; }

            public string Company { get; set; }

            public string Environment { get; set; }

            [DataType(DataType.Date)]
            public DateTime CreatedDate { get; set; }

            public string JobSeekerId { get; set; }

            public string ActionResult { get; set; }

            public string AnswerList { get; set; }

            [ScaffoldColumn(false)]
        public DateTime? DateTo { get; set; }
          [ScaffoldColumn(false)]
        public DateTime? DateFrom { get; set; }
     }
}