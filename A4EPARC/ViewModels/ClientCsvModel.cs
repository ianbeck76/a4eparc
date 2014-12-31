using System;
using System.ComponentModel.DataAnnotations;

namespace A4EPARC.ViewModels
{
    public class ClientCsvModel
    {
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        public int MatrixPreContemplationPoints { get; set; }
        public int MatrixContemplationPoints { get; set; }
        public int MatrixActionPoints { get; set; }
        public int PreContemplationPoints { get; set; }
        public int ContemplationPoints { get; set; }
        public int ActionPoints { get; set; }
        public string ActionName { get; set; }
        public string AnswerString { get; set; }
        public string Username { get; set; }
        public string Company { get; set; }
        public string CaseWorkerName { get; set; }
        public string CaseWorkerId { get; set; }
        public string Surname { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string LengthOfUnemployment { get; set; }
        public string Comments { get; set; }
        public string CaseId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}