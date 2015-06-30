using System;
using System.ComponentModel.DataAnnotations;

namespace A4EPARC.ViewModels
{
    [Serializable]
    public class ClientQueryViewModel
    {
        public DateTime? DateTo { get; set; }
        public DateTime? DateFrom { get; set; }
        public string Username { get; set; }
        public string Company { get; set; }
        public string JobSeekerID { get; set; }
        public string Surname { get; set; }
    }
}