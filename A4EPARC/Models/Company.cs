namespace A4EPARC.Models
{
    public class Company : Entity, ICompany
    {
        public string Name { get; set; }
    }

    public interface ICompany : IEntity
    {
        string Name { get; set; }
    }
}