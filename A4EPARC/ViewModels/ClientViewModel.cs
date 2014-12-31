using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using A4EPARC.Enums;

namespace A4EPARC.ViewModels
{
    public class ClientViewModel
    {
        public ClientViewModel() 
        {
            Questions = new List<QuestionViewModel>();
            SiteLabels = new List<SiteLabelsViewModel>();
        }

        public List<SiteLabelsViewModel> SiteLabels { get; set; }

        public int Id { get; set; }

        public string Next { get; set; }

        public int ResultId { get; set; }

        [Display(Name = "Created Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime CreatedDate { get; set; }
        
        public IEnumerable<QuestionViewModel> Questions { get; set; }

        public bool Deleted { get; set; }

        public ResultViewModel Result { get; set; }

        [Display(Name = "PreContemplation Matrix Score")]
        public int MatrixPreContemplationPoints { get; set; }

        [Display(Name="Contemplation Matrix Score")]
        public int MatrixContemplationPoints{ get; set; }

        [Display(Name = "Action Matrix Score")]
        public int MatrixActionPoints { get; set; }

        [Display(Name = "PreContemplation Score")]
        public int PreContemplationPoints { get; set; }

        [Display(Name = "Contemplation Score")]
        public int ContemplationPoints { get; set; }

        [Display(Name = "Action Score")]
        public int ActionPoints { get; set; }

        public int ActionIdToDisplay { get; set; }

        public string ActionTypeName { get; set; }

        public string ActionTypeDescription { get; set; }

        public string ActionTypeSummary { get; set; }

        public string AnswerString { get; set; }

       //[Required]
        [Display(Name = "Accept Checkbox")]
        public bool AcceptCheckBox { get; set; }

        public int UserId { get; set; }

        //[Required]
        [Display(Name = "Agency")]
        public string Agency { get; set; }

        [Display(Name = "Company")]
        public string Company { get; set; }

        [Display(Name = "How many times has the client completed this survey before?")]
        public int? NumberOfTimesSurveyCompleted { get; set; }

        //[Required]
        [Display(Name = "Case Worker Name")]
        public string CaseWorkerName { get; set; }

        //[Required]
        [Display(Name = "Case Worker ID")]
        public string CaseWorkerId { get; set; }

        [Display(Name = "Stream (if known)")]
        public string Stream { get; set; }

        public IEnumerable<string> StreamDropdownList { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        //[Required]
        [Display(Name = "Client Last Name")]
        public string Surname { get; set; }

        //[Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "Male")]
        public bool Male { get; set; }

        [Display(Name = "Female")]
        public bool Female { get; set; }

        [Display(Name = "Year")]
        public int DateOfBirthYear { get; set; }

        [Display(Name = "Month")]
        public int DateOfBirthMonth { get; set; }

        [Display(Name = "Day")]
        public int DateOfBirthDay { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        public IEnumerable<int> DayDropdownList { get; set; }

        public IEnumerable<KeyValuePair<int, string>> MonthDropdownList { get; set; }

        public IEnumerable<int> YearDropdownList { get; set; }

        [Display(Name = "Length of Unemployment (Months)")]
        public string LengthOfUnemployment { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        public IEnumerable<string> LengthOfUnemploymentDropdownList { get; set; }

        [Display(Name = "Job Seeker ID")]
        public string JobSeekerID { get; set; }

        [Display(Name = "Completed all 5 Workshops?")]
        public bool CompletedAll5Pathways { get; set; }

        [Display(Name = "Scheme")]
        public int SchemeId { get; set; }

        [Display(Name = "Scheme?")]
        public IEnumerable<KeyValuePair<int, string>> SchemeDropdownList { get; set; }

        //[Required]
        [Display(Name = "Case Number")]
        public string CaseId { get; set; }


    }
}