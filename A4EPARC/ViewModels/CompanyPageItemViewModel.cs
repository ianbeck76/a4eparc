namespace A4EPARC.ViewModels
{
    public class CompanyPageItemViewModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public bool? IsDisplay { get; set; }
        public bool? IsRequired { get; set; }
    }
}