namespace A4EPARC.Services
{
    public class Service<T> : IService<T> where T : class
    {
        public Service()
        {
        }
    }

    public interface IService<T>
    {
    }
}
