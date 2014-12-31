using System.Data;
using System.Data.SqlClient;

namespace A4EPARC.Repositories
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }

    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}