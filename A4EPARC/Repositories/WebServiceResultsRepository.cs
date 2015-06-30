using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using A4EPARC.ViewModels;

namespace A4EPARC.Repositories
{
    public class WebServiceResultsRepository : Repository<WebServiceResultsViewModel>, IWebServiceResultsRepository
    {
        #region Public Methods

        public IQueryable<WebServiceResultsViewModel> GetResultsView(DateTime? dateFrom, DateTime? dateTo, string jobseekerid, string environment, string company)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                var cmd = connection.CreateCommand();

                var modelList = new List<WebServiceResultsViewModel>();

                var query = @"select w.Id, c.Name AS Company, k.Type AS Environment, w.CreatedDate, w.JobSeekerID, 
                            a.ActionType AS ActionResult, 
                            w.AnswerString AS AnswerList
                            FROM WebServiceLog w INNER JOIN WebServiceKeys k ON k.UniqueKey = w.UniqueKey
                            INNER JOIN CompanyNew c ON c.Id = k.CompanyId
                            INNER JOIN ActionType a ON a.Id = w.ActionTypeId";

                var clause = " WHERE ";

                if (!string.IsNullOrWhiteSpace(jobseekerid))
                {
                    query += clause += "w.JobSeekerID = @JobseekerId";
                    clause = " AND ";
                }
                if (!string.IsNullOrWhiteSpace(environment))
                {
                    query += clause += "k.Type = @Environment";
                    clause = " AND ";
                }
                if (!string.IsNullOrWhiteSpace(company))
                {
                    query += clause += "c.Name = @Company";
                    clause = " AND ";
                }
                if (dateFrom.HasValue)
                {
                    query += clause += "w.CreatedDate > @DateFrom";
                    clause = " AND ";
                }
                if (dateTo.HasValue)
                {
                    query += clause += "w.CreatedDate < @DateTo";
                    clause = " AND ";
                }
                query += " ORDER BY w.CreatedDate DESC";

                var model = new WebServiceResultsViewModel { JobSeekerId = jobseekerid, Environment = environment, DateFrom = dateFrom, DateTo = dateTo, Company = company };

                return Query<WebServiceResultsViewModel>(query, model).AsQueryable();
            }
        }

        public List<WebServiceCsvExportViewModel> GetCsvData(DateTime? toDate, DateTime? fromDate, string environmentType, string jobSeekerID, string company)
        {
            using (var connection = DbConnectionFactory.CreateConnection())
            {
                connection.Open();
                var cmd = connection.CreateCommand();

                var results = new List<WebServiceCsvExportViewModel>();

                var commandText = "select w.Id, c.Name, k.Type, w.CreatedDate, w.JobSeekerID, w.ActionTypeId, w.AnswerString ";
                commandText += "FROM WebServiceLog w INNER JOIN WebServiceKeys k ON k.UniqueKey = w.UniqueKey ";
                commandText += "INNER JOIN CompanyNew c ON c.Id = k.CompanyId";

                if (toDate.HasValue)
                {
                    commandText += " WHERE w.CreatedDate < @ToDate";
                }
                if (fromDate.HasValue)
                {
                    commandText += toDate.HasValue ? " AND w.CreatedDate > @FromDate" : " WHERE w.CreatedDate > @FromDate";
                }
                if (!string.IsNullOrWhiteSpace(environmentType) && environmentType != "BOTH")
                {
                    commandText += toDate.HasValue || fromDate.HasValue ? " AND k.Type = @EnvironmentType" : " WHERE k.Type = @EnvironmentType";
                }
                if (!string.IsNullOrWhiteSpace(jobSeekerID))
                {
                    commandText += toDate.HasValue || fromDate.HasValue || !string.IsNullOrWhiteSpace(environmentType) ? " AND w.JobSeekerID = @JobSeekerID" : " WHERE w.JobSeekerID = @JobSeekerID";
                }
                cmd.CommandText = commandText;

                cmd.Prepare();

                cmd.Parameters.Add(new { @ExpiryDate = DateTime.UtcNow.AddMonths(-1).AddDays(-1)});
                //TODO Put back
                //cmd.Parameters.AddWithValue("@FromDate", SqlDbType.DateTime).Value = fromDate ??
                //                                                                     DateTime.Now.AddYears(-100);
                //cmd.Parameters.AddWithValue("@ToDate", SqlDbType.DateTime).Value = toDate ?? DateTime.Now.AddYears(100);
                //cmd.Parameters.AddWithValue("@EnvironmentType", SqlDbType.VarChar).Value = string.IsNullOrWhiteSpace(environmentType) || environmentType == "BOTH" ? "" : environmentType == "LIVE" ? "PROD" : "TEST";
                //cmd.Parameters.AddWithValue("@JobSeekerID", SqlDbType.VarChar).Value = jobSeekerID ?? "";

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var result = new WebServiceCsvExportViewModel();
                    result.Id = reader.GetInt32(0);
                    result.CompanyName = reader.GetString(1);
                    var environment = reader.GetString(2);
                    if (environment == "PROD")
                    {
                        environment = "LIVE";
                    }
                    result.EnvironmentType = environment;
                    result.CreatedDate = reader.GetDateTime(3);
                    result.JobSeekerID = !reader.IsDBNull(4) ? (reader.GetString(4)) : "";
                    result.ActionToDisplay = CalculateAction(reader.GetInt16(5));

                    if (!reader.IsDBNull(6))
                    {
                        var answerString = reader.GetString(6);

                        if (answerString.Length > 22)
                        {
                            result.QuestionOne = answerString.Substring(0, 1);
                            result.QuestionTwo = answerString.Substring(2, 1);
                            result.QuestionThree = answerString.Substring(4, 1);
                            result.QuestionFour = answerString.Substring(6, 1);
                            result.QuestionFive = answerString.Substring(8, 1);
                            result.QuestionSix = answerString.Substring(10, 1);
                            result.QuestionSeven = answerString.Substring(12, 1);
                            result.QuestionEight = answerString.Substring(14, 1);
                            result.QuestionNine = answerString.Substring(16, 1);
                            result.QuestionTen = answerString.Substring(18, 1);
                            result.QuestionEleven = answerString.Substring(20, 1);
                            result.QuestionTwelve = answerString.Substring(22, 1);

                        }

                    }
                    results.Add(result);
                }
                return results;
            }
        }

        #endregion
    }

    public interface IWebServiceResultsRepository : IRepository<WebServiceResultsViewModel>
    {
        IQueryable<WebServiceResultsViewModel> GetResultsView(DateTime? dateFrom, DateTime? dateTo, string jobseekerid, string environment, string company);

        List<WebServiceCsvExportViewModel> GetCsvData(DateTime? toDate, DateTime? fromDate, string environmentType,
                                                      string jobSeekerID, string company);
    }
}