using System;
using System.ComponentModel.DataAnnotations;

namespace A4EPARC.ViewModels
{
    public class WebServiceCsvExportViewModel
    {
        public int Id { get; set; }

        public string CompanyName { get; set; }

        public string JobSeekerID { get; set; }

        public string EnvironmentType { get; set; }

        public string ActionToDisplay { get; set; }

        public string QuestionOne { get; set; }

        public string QuestionTwo { get; set; }

        public string QuestionThree { get; set; }

        public string QuestionFour { get; set; }

        public string QuestionFive { get; set; }

        public string QuestionSix { get; set; }

        public string QuestionSeven { get; set; }

        public string QuestionEight { get; set; }

        public string QuestionNine { get; set; }

        public string QuestionTen { get; set; }

        public string QuestionEleven { get; set; }

        public string QuestionTwelve { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
    }
}