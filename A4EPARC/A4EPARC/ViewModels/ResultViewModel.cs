using System.ComponentModel.DataAnnotations;
using A4EPARC.Enums;

namespace A4EPARC.ViewModels
{
    public class ResultViewModel
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        [Display(Name = "PreContemplation Matrix Score")]
        public int PreContemplationScoreMatrix { get; set; }

        [Display(Name="Contemplation Matrix Score")]
        public int ContemplationScoreMatrix { get; set; }

        [Display(Name = "Action Matrix Score")]
        public int ActionScoreMatrix { get; set; }

        [Display(Name = "PreContemplation Score")]
        public int PreContemplationScore { get; set; }

        [Display(Name = "Contemplation Score")]
        public int ContemplationScore { get; set; }

        [Display(Name = "Action Score")]
        public int ActionScore { get; set; }

        public ActionType ActionIdToDisplay { get; set; }

        public string AnswerString { get; set; }

    }
}