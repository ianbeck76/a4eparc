namespace A4EPARC.Models
{
    public class Entity : IEntity
    {
        public int Id { get; set; }
    }

    public interface IEntity
    {
        int Id { get; set; }
    }

}