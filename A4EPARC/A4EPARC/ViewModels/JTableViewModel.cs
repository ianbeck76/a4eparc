using A4EPARC.Models;

namespace A4EPARC.ViewModels
{
    public class JTableViewModel : Entity
    {
        public int StartIndex { get; set; }
        public string Sorting { get; set; }
        public string Details { get; set; }
    }
}