namespace A4EPARC.ViewModels
{
    public class UserViewModel : JTableViewModel
    {
        public string Email { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsViewer { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}