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

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime CreatedDate { get; set; }
        
        public IEnumerable<QuestionViewModel> Questions { get; set; }

        public bool Deleted { get; set; }

        public ResultViewModel Result { get; set; }

        public int MatrixPreContemplationPoints { get; set; }

        public int MatrixContemplationPoints{ get; set; }

        public int MatrixActionPoints { get; set; }

        public int PreContemplationPoints { get; set; }

        public int ContemplationPoints { get; set; }

        public int ActionPoints { get; set; }

        public int ActionIdToDisplay { get; set; }

        public string ActionTypeName { get; set; }

        public string AnswerString { get; set; }

        public bool AcceptCheckBox { get; set; }

        public int UserId { get; set; }

        public string Agency { get; set; }

        public string Company { get; set; }

        public int? HowManyTimesHasSurveyBeenCompleted { get; set; }

        public string CaseWorkerName { get; set; }

        public string CaseWorkerId { get; set; }

        public string CaseId { get; set; }

        public string Stream { get; set; }

        public IEnumerable<string> StreamDropdownList { get; set; }

        public string Project { get; set; }

        public IEnumerable<string> ProjectDropdownList { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string Gender { get; set; }

        [DisplayFormat(NullDisplayText = "N/A")]
        public string EnrolmentID { get; set; }

        public bool Male { get; set; }

        public bool Female { get; set; }

        public int DateOfBirthYear { get; set; }

        public int DateOfBirthMonth { get; set; }

        public int DateOfBirthDay { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DateOfBirth { get; set; }

        public IEnumerable<int> DayDropdownList { get; set; }

        public IEnumerable<KeyValuePair<int, string>> MonthDropdownList { get; set; }

        public IEnumerable<int> YearDropdownList { get; set; }

        public string Comments { get; set; }

        public string LengthOfUnemployment { get; set; }

        public IEnumerable<string> LengthOfUnemploymentDropdownList { get; set; }

        public string State { get; set; }

        public IEnumerable<string> StateDropdownList { get; set; }

        public string JobSeekerID { get; set; }

        [RegularExpression("^[wW]{1}[pP]{1}[0-9]{6}[a-zA-Z]{1}$")]
        public string CustomerId { get; set; }

        public int SchemeId { get; set; }

        public IEnumerable<KeyValuePair<int, string>> SchemeDropdownList { get; set; }

        public string AdvisorName {get;set;}

        public string Organisation {get;set;}

        public bool CompletedAllFiveWorkshops {get;set;}

        public IEnumerable<CompanyPageItemViewModel> PageItems { get; set; }

        public int NumberOfPreviousAttempts { get; set; }

        public bool IsIslander { get; set; }

        public string RTO { get; set; }

        public IEnumerable<string> RTODropdownList { get; set; }

        public string Provider { get; set; }

        public IEnumerable<string> ProviderDropdownList { get; set; }

        public bool? IsCurrentlyCollectingBenefits { get; set; }

        public string UnemploymentInsuranceId { get; set; }

        [EmailAddress]
        public string CustomerEmail { get; set; }

        [RegularExpression("([0-9]+)")]
        public string UnemploymentInsurance1 { get; set; }

        [RegularExpression("([0-9]+)")]
        public string UnemploymentInsurance2 { get; set; }

        [RegularExpression("([0-9]+)")]
        public string UnemploymentInsurance3 { get; set; }

        [RegularExpression("([0-9]+)")]
        public string UnemploymentInsurance4 { get; set; }

        public bool? IsOverEighteen { get; set; }

        public bool? HasDiplomaOrGED { get; set; }

        public bool? IsReassessment { get; set; }

        public string CustomerCaseNumber { get; set; }

        [RegularExpression("([a-zA-Z]+)")]
        public string CustomerCaseNumber1 { get; set; }
        
        [RegularExpression("([0-9]+)")]
        public string CustomerCaseNumber2 { get; set; }
        
        [RegularExpression("([0-9]+)")]
        public string CustomerCaseNumber3 { get; set; }

        [RegularExpression("([0-9]+)")]
        public string CustomerCaseNumber4 { get; set; }

        public IEnumerable<string> MaritalStatusDropdownList { get; set; }
        public string MaritalStatus { get; set; }

        public IEnumerable<string> NumberOfChildrenDropdownList { get; set; }
        public string NumberOfChildren { get; set; }

        public DateTime? EmailSentDate { get; set; }

    }
}