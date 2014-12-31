using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Dapper;

namespace A4EPARC.Repositories
{
    public class Repository<T> : IDisposable, IRepository<T> where T : class
    {
        public Repository()
        {
            DbConnectionFactory = new SqlConnectionFactory(ConfigurationManager.ConnectionStrings["SOC"].ConnectionString);
        }

        public Repository(string connectionString)
        {
            DbConnectionFactory = new SqlConnectionFactory(connectionString);
        }

        internal string TableName
        {
            get
            {
                if (typeof(T).Name.Contains("ViewModel"))
                {
                    return typeof(T).Name.Replace("ViewModel", "").ToUpper(); 
                }
                return typeof(T).Name.ToUpper();
            }
        }

        protected readonly IDbConnectionFactory DbConnectionFactory;

        public System.Collections.Generic.IEnumerable<T> Query(string query, object parameters)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                return connection.Query<T>(query, parameters);
            }
        }

        public IEnumerable<T> Query(string query)
        {
            return Query(query, null);
        }

        protected IEnumerable<G> Query<G>(string query)
        {
            return Query<G>(query, null);
        }

        protected IEnumerable<G> Query<G>(string query, object parameters)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                return connection.Query<G>(query, parameters);
            }
        }

        protected IEnumerable<G> Query<G>(string query, object parameters, CommandType commandType)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                return connection.Query<G>(query, parameters, commandType: commandType);
            }
        }

        public virtual T SingleOrDefaultByReportId(int reportId)
        {
            return Query<T>("SELECT * FROM [dbo].[" + TableName + "] WHERE ReportId = @ReportId", new { ReportId = reportId }).FirstOrDefault();
        }

        public virtual T SingleOrDefaultByIdentifier(int identifier)
        {
            return Query<T>("SELECT * FROM [dbo].[" + TableName + "] WHERE Identifier = @Identifier", new { Identifier = identifier }).FirstOrDefault();
        }

        public T SingleOrDefault(string where, object parameters)
        {
            return Where(where, parameters).SingleOrDefault();
        }

        public IEnumerable<T> Where(string where, object parameters)
        {
            return Query<T>("SELECT * FROM [dbo].[" + TableName + "] WHERE " + where, parameters);
        }

        public IEnumerable<T> All()
        {
            return Query<T>("SELECT * FROM [dbo].[" + TableName + "]");
        }

        public virtual IEnumerable<T> All(int reportId)
        {
            return Query<T>("SELECT * FROM [dbo].[" + TableName + "] WHERE ReportId = @Id", new { Id = reportId });
        }

        public T Single(int id)
        {
            return Query<T>("SELECT * FROM [dbo].[" + TableName + "] WHERE Id = @Id", new { Id = id }).Single();
        }

        public T SingleOrDefault(int id)
        {
            return Query<T>("SELECT * FROM [dbo].[" + TableName + "] WHERE Id = @Id", new { Id = id }).SingleOrDefault();
        }

        public int Remove(T obj)
        {
            return Execute("DELETE FROM [dbo].[" + this.TableName + "] WHERE Id = @Id", obj);
        }

        public virtual int Remove(int id)
        {
            return Execute("DELETE FROM [dbo].[" + this.TableName + "] WHERE Id = @id", new { id = id });
        }

        protected int Execute(string query, object parameters)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                return connection.Execute(query, parameters);
            }
        }
        protected int Execute(string query, object parameters, CommandType commandType)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                return connection.Execute(query, parameters, commandType: commandType);
            }
        }

        protected int Execute(string query)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                return connection.Execute(query);
            }
        }

        // Flag: Has Dispose already been called? 
        bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {

        }

        protected string CalculateAction(int actionTypeId)
        {
            string action = "None";
            switch (actionTypeId)
            {
                case 1:
                    action = "PreContemplation";
                    break;
                case 2:
                    action = "Unauthentic Action";
                    break;
                case 3:
                    action = "Contemplation";
                    break;
                case 4:
                    action = "Preparation";
                    break;
                case 5:
                    action = "Action";
                    break;
            }
            return action;
        }

    }

    public interface IRepository<T>
    {
        T SingleOrDefault(string where, object parameters);
        IEnumerable<T> Query(string query, object parameters);
        IEnumerable<T> Query(string query);
        T SingleOrDefault(int id);
        T SingleOrDefaultByReportId(int id);
        T Single(int id);
        IEnumerable<T> All();
        IEnumerable<T> All(int id);
        int Remove(T obj);
        int Remove(int id);
    }
}
