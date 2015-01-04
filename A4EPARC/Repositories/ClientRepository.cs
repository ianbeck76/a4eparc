using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using A4EPARC.ViewModels;

namespace A4EPARC.Repositories
{
    public class ClientRepository : Repository<ClientViewModel>, IClientRepository
    {
        public int InsertPerson(ClientViewModel model)
        {
            const string query = @"INSERT INTO Client (FirstName,Surname,
            DateOfBirth,CaseWorkerName,CaseWorkerId,Agency,Gender,CaseId,
            LengthOfUnemployment,Comments,CreatedDate,UserId,Deleted,SchemeId,State)
            VALUES(@FirstName,@Surname,@DateOfBirth,
            @CaseWorkerName,@CaseWorkerId,@Agency,@Gender,@CaseId,@LengthOfUnemployment,
            @Comments,@CreatedDate,@UserId,@Deleted,@SchemeId,@State);
            SELECT CAST(SCOPE_IDENTITY() as int);";

            return Query<int>(query, model).SingleOrDefault();
        }

        public int InsertResult(ResultViewModel model)
        {
            const string query = @"INSERT INTO ClientResult(
            ClientId,ActionPoints,ContemplationPoints,PreContemplationPoints,
            MatrixActionPoints,MatrixContemplationPoints,MatrixPreContemplationPoints,
            ActionIdToDisplay,AnswerString)
            VALUES(@ClientId,@ActionScore,@ContemplationScore,@PreContemplationScore,
            @ActionScoreMatrix,@ContemplationScoreMatrix,@PreContemplationScoreMatrix,
            @ActionIdToDisplay,@AnswerString);
            SELECT CAST(SCOPE_IDENTITY() as int);";

            return Query<int>(query, model).SingleOrDefault();
        }

        public ClientViewModel GetClient(int id)
        {
            const string query = @"select c.Id,
                                c.FirstName,                                
                                c.Surname,
                                c.DateOfBirth, 
                                c.SchemeId,
                                c.UserId,
                                c.Agency, 
                                c.CaseId, 
                                c.CaseWorkerId, 
                                c.CaseWorkerName, 
                                c.Comments, 
                                c.Gender,
                                c.LengthOfUnemployment,
                                c.State,
                                c.Accepted, 
                                cr.ActionIdToDisplay, 
                                cr.ActionPoints, 
                                cr.ContemplationPoints, 
                                cr.PreContemplationPoints,
                                cr.MatrixActionPoints, 
                                cr.MatrixContemplationPoints, 
                                cr.MatrixprecontemplationPoints,
                                cr.AnswerString,
                                cr.Id AS ResultId
                                FROM Client c 
                                LEFT OUTER JOIN ClientResult cr ON cr.ClientId = c.Id
                                WHERE c.Id = @Id";

            return Query(query, new {id}).FirstOrDefault();
        }

        public IQueryable<ClientResultViewModel> All(DateTime? dateFrom, DateTime? dateTo, string caseId, string clientId)
        {
            var query = string.Format(@"select c.Id, c.CaseId AS CaseID, 
                                            u.Email AS Username, 
                                            c.CreatedDate,
                                            a.ActionType AS ActionName, 
                                            cr.AnswerString,
                                            cr.PreContemplationPoints,
                                            cr.MatrixPreContemplationPoints,
                                            cr.ContemplationPoints,
                                            cr.MatrixContemplationPoints,
                                            cr.ActionPoints,
                                            cr.MatrixActionPoints
                                            FROM [dbo].[Client] c 
                                            INNER JOIN [dbo].[ClientResult] cr ON cr.ClientId = c.Id
                                            INNER JOIN [dbo].[ActionType] a ON a.Id = cr.ActionIdToDisplay
                                            INNER JOIN [dbo].[User] u ON c.UserId = u.Id");
            if (!string.IsNullOrWhiteSpace(caseId))
            {
                query += string.Format(" AND c.CaseId = @CaseId");
            }
            if (dateFrom.HasValue)
            {
                query += " AND c.CreatedDate > @DateFrom";
            }
            if (dateTo.HasValue)
            {
                query += " AND c.CreatedDate < @DateTo";
            }
            query += " ORDER BY c.CreatedDate DESC";

            var model = new ClientResultViewModel {CaseID = caseId, DateFrom = dateFrom, DateTo = dateTo };
            
            return Query<ClientResultViewModel>(query, model).AsQueryable();
        }

        public IList<ClientCsvModel> GetCsvData(DateTime? dateFrom, DateTime? dateTo, string caseId)
        {
            var query = string.Format(@"select c.Id, c.CaseId,
                                            c.Comments,
                                            c.DateOfBirth,
                                            c.Surname,
                                            c.CaseWorkerId,
                                            c.CaseWorkerName, 
                                            u.Email AS Username, 
                                            c.CreatedDate,
                                            c.LengthOfUnemployment,
                                            c.Gender,  
                                            a.ActionType AS ActionName, 
                                            cr.AnswerString,
                                            cr.PreContemplationPoints,
                                            cr.MatrixPreContemplationPoints,
                                            cr.ContemplationPoints,
                                            cr.MatrixContemplationPoints,
                                            cr.ActionPoints,
                                            cr.MatrixActionPoints
                                            FROM [dbo].[Client] c 
                                            INNER JOIN [dbo].[ClientResult] cr ON cr.ClientId = c.Id
                                            INNER JOIN [dbo].[ActionType] a ON a.Id = cr.ActionIdToDisplay
                                            INNER JOIN [dbo].[User] u ON c.UserId = u.Id");
            if (!string.IsNullOrWhiteSpace(caseId))
            {
                query += string.Format(" AND c.CaseId = @CaseId");
            }
            if (dateFrom.HasValue)
            {
                query += " AND c.CreatedDate > @DateFrom";
            }
            if (dateTo.HasValue)
            {
                query += " AND c.CreatedDate < @DateTo";
            }
            query += " ORDER BY c.CreatedDate DESC";

            var model = new ClientCsvModel { CaseId = caseId, DateFrom = dateFrom, DateTo = dateTo };

            return Query<ClientCsvModel>(query, model).ToList();
        }

        public bool CheckReference(string reference)
        {
            return false;
            //TODO finsih this off
            //var query = @"select c.Reference FROM Client c INNER JOIN ClientResult cr ON cr.ClientId = c.Id WHERE c.Reference = @Reference";

            //cmd.Prepare();

            //cmd.Parameters.Add(new { @Reference = reference });

            //var i = 1;

            //var reader = cmd.ExecuteReader();
            //if (reader.RecordsAffected >0)
            //{
            //    while (reader.Read())
            //    {
            //        i++;
            //    }
            //}

            //return i > Convert.ToInt32(WebConfigurationManager.AppSettings["NumberOfAttemptsAllowed"]);
        }
    }

    public interface IClientRepository : IRepository<ClientViewModel>
    {
        int InsertPerson(ClientViewModel client);
        int InsertResult(ResultViewModel result);
        ClientViewModel GetClient(int id);
        IQueryable<ClientResultViewModel> All(DateTime? dateFrom, 
        DateTime? dateTo, string caseId, string clientId);
        IList<ClientCsvModel> GetCsvData(DateTime? dateFrom, DateTime? dateTo, string caseId);

    }
}
