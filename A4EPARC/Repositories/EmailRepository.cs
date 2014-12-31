using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace A4EPARC.Repositories
{
    public class EmailRepository : Repository<string>, IEmailRepository
    {
        public int GetEmailLogCount()
        {
            return
                Query<int>(
                    "select count(CreatedDate) FROM EmailLog where CreatedDate = @CreatedDate",
                    new { CreatedDate = DateTime.UtcNow.Date}).First();
        }

        public void InsertEmailLog()
        {
            Execute("INSERT INTO EmailLog (CreatedDate) VALUES(@CreatedDate)",
                          new {CreatedDate = DateTime.UtcNow.Date});
        }
    }

    public interface IEmailRepository
    {
        int GetEmailLogCount();
        void InsertEmailLog();
    }
}