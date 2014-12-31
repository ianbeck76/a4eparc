using System;

namespace A4EPARC.Models
{
    public class User : Entity, IUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsViewer { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public interface IUser : IEntity
    {
        string Email { get; set; }
        string Password { get; set; }
        string Salt { get; set; }
        bool IsSuperAdmin { get; set; }
        bool IsAdmin { get; set; }
        bool IsViewer { get; set; }
        int CompanyId { get; set; }
        DateTime CreatedDate { get; set; }
    }
}