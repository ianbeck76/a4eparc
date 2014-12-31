namespace A4EPARC.ViewModels
{
    public class QuestionViewModel : JTableViewModel
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public int ActionTypeId { get; set; }

        public int? Answer { get; set; }

        public int? SchemeId { get; set; }

        public string LanguageCode { get; set; }
    }
}