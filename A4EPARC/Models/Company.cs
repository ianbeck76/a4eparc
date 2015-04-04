namespace A4EPARC.Models
{
    public class Company : Entity, ICompany
    {
        public string Name { get; set; }
        public string EmailFromAddress { get; set; }
        public string DefaultPassword { get; set; }
    }

    public interface ICompany : IEntity
    {
        string Name { get; set; }
        string EmailFromAddress { get; set; }
        string DefaultPassword { get; set; }
    }
}