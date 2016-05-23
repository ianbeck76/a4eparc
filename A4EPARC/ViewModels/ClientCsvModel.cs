using System;
using System.ComponentModel.DataAnnotations;

namespace A4EPARC.ViewModels
{
    public class ClientCsvModel
    {
        [DataType(DataType.Date)]
        public string Company { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Username { get; set; }
        public string Agency { get; set; }
        public string ActionName { get; set; }
        public string AnswerString { get; set; }
        public int MatrixPreContemplationPoints { get; set; }
        public int MatrixContemplationPoints { get; set; }
        public int MatrixActionPoints { get; set; }
        public int PreContemplationPoints { get; set; }
        public int ContemplationPoints { get; set; }
        public int ActionPoints { get; set; }
        public string CaseId { get; set; }
        public string CaseWorkerName { get; set; }
        public string CaseWorkerId { get; set; }
        public string Comments { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EnrolmentID { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public string LengthOfUnemployment { get; set; }
        public int HowManyTimesHasSurveyBeenCompleted { get; set; }
        public string JobSeekerID { get; set; }
        public string SchemeName { get; set; }
        public string Surname { get; set; }
        public string State { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Stream { get; set; }
        public string CompletedAllFiveWorkshops { get; set; }
        public string IsIslander { get; set; }
        public string RTO { get; set; }
        public string IsCurrentlyCollectingBenefits { get; set; }
        public string UnemploymentInsuranceId { get; set; }
        public string IsOverEighteen { get; set; }
        public string HasDiplomaOrGED { get; set; }
        public string IsReassessment { get; set; }
        public string Provider { get; set; }
        public string Project { get; set; }
        public string CustomerCaseNumber { get; set; }
        public string MaritalStatus { get; set; }
        public string NumberOfChildren { get; set; }
        public string CustomerId { get; set; }
        public string CustomerEmail { get; set; }
    }
}