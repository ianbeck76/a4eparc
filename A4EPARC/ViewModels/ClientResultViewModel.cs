using System;
using System.ComponentModel.DataAnnotations;

namespace A4EPARC.ViewModels
{
    [Serializable]
    public class ClientResultViewModel
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? DateTo { get; set; }
          [ScaffoldColumn(false)]
        public DateTime? DateFrom { get; set; }
          [DataType(DataType.Date)]
          public DateTime CreatedDate { get; set; }
        public string ActionName { get; set; }
        public string AnswerString { get; set; }
        public string Username { get; set; }
        [ScaffoldColumn(false)]
        public string Company { get; set; }
        public string JobSeekerID { get; set; }
        public string Surname { get; set; }
        [ScaffoldColumn(false)]
        public int MatrixPreContemplationPoints { get; set; }
        [ScaffoldColumn(false)]
        public int MatrixContemplationPoints { get; set; }
        [ScaffoldColumn(false)]
        public int MatrixActionPoints { get; set; }
        [ScaffoldColumn(false)]
        public int PreContemplationPoints { get; set; }
        [ScaffoldColumn(false)]
        public int ContemplationPoints { get; set; }
        [ScaffoldColumn(false)]
        public int ActionPoints { get; set; }
        public bool? Deleted { get; set; }
    }
}