namespace A4EPARC.ViewModels
{
    public class SiteTextViewModel : JTableViewModel
    {
        public int Code { get; set; }

        public int? SchemeId { get; set; }

        public string LanguageCode { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public int? OrderNumber { get; set; }

    }
}