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
            const string query = @"INSERT INTO ClientNew 
            (FirstName
            ,Surname
            ,DateOfBirth
            ,CaseWorkerName
            ,CaseWorkerId
            ,CaseId
            ,Agency
            ,Gender
            ,LengthOfUnemployment
            ,CreatedDate
            ,UserId
            ,Deleted
            ,SchemeId
            ,State
            ,AdvisorName
            ,Organisation
            ,Stream
            ,CompletedAllFiveWorkshops
            ,Comments
            ,EnrolmentID
            ,HowManyTimesHasSurveyBeenCompleted
            ,JobSeekerID
            ,IsIslander
            ,RTO
            ,IsCurrentlyCollectingBenefits
            ,UnemploymentInsuranceId
            ,IsOverEighteen
            ,HasDiplomaOrGED
            ,Provider
            ,IsReassessment
            ,Project)
            VALUES(@FirstName
            ,@Surname
            ,@DateOfBirth
            ,@CaseWorkerName
            ,@CaseWorkerId
            ,@CaseId
            ,@Agency
            ,@Gender
            ,@LengthOfUnemployment
            ,@CreatedDate
            ,@UserId
            ,@Deleted
            ,@SchemeId
            ,@State
            ,@AdvisorName
            ,@Organisation
            ,@Stream
            ,@CompletedAllFiveWorkshops
            ,@Comments
            ,@EnrolmentID
            ,@HowManyTimesHasSurveyBeenCompleted
            ,@JobSeekerID
            ,@IsIslander
            ,@RTO
            ,@IsCurrentlyCollectingBenefits
            ,@UnemploymentInsuranceId
            ,@IsOverEighteen
            ,@HasDiplomaOrGED
            ,@Provider
            ,@IsReassessment
            ,@Project);
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
            const string query = @"select c.Id
                                ,c.FirstName
                                ,c.Surname
                                ,c.DateOfBirth
                                ,c.CaseWorkerName
                                ,c.CaseWorkerId
                                ,c.CaseId
                                ,c.Agency
                                ,c.Gender
                                ,c.JobSeekerID
                                ,c.LengthOfUnemployment
                                ,c.Comments
                                ,c.CreatedDate
                                ,c.UserId
                                ,c.Deleted
                                ,c.SchemeId
                                ,c.State
                                ,c.AdvisorName
                                ,c.Organisation
                                ,c.Stream
                                ,c.CompletedAllFiveWorkshops
                                ,c.HowManyTimesHasSurveyBeenCompleted
                                ,c.EnrolmentID
                                ,cr.ActionIdToDisplay 
                                ,cr.ActionPoints 
                                ,cr.ContemplationPoints 
                                ,cr.PreContemplationPoints
                                ,cr.MatrixActionPoints
                                ,cr.MatrixContemplationPoints
                                ,cr.MatrixprecontemplationPoints
                                ,cr.AnswerString
                                ,cr.Id AS ResultId
                             	,com.Name AS Company
                                ,c.IsIslander
                                ,c.RTO
                                ,c.IsCurrentlyCollectingBenefits
                                ,c.UnemploymentInsuranceId
                                ,c.IsOverEighteen
                                ,c.HasDiplomaOrGED
                                ,c.Provider
                                ,c.IsReassessment
                                ,c.Project
								FROM ClientNew c 
                                INNER JOIN [user] u ON u.id = c.UserId
								INNER JOIN [CompanyNew] com ON com.Id = u.CompanyId
                                LEFT OUTER JOIN ClientResult cr ON cr.ClientId = c.Id
                                WHERE c.Id = @Id";

            return Query(query, new {id}).FirstOrDefault();
        }

        public IQueryable<ClientResultViewModel> All(DateTime? dateFrom, DateTime? dateTo, string jobSeekerID, string surname, string username, string company)
        {
            var query = string.Format(@"select c.Id, c.JobSeekerID, c.Surname,
                                            u.Email AS Username, 
                                            c.CreatedDate,
                                            a.ActionType AS ActionName, 
                                            cr.AnswerString
                                            FROM [dbo].[ClientNew] c 
                                            INNER JOIN [dbo].[ClientResult] cr ON cr.ClientId = c.Id
                                            INNER JOIN [dbo].[ActionType] a ON a.Id = cr.ActionIdToDisplay
                                            INNER JOIN [dbo].[User] u ON c.UserId = u.Id
                                            INNER JOIN [dbo].[CompanyNew] com ON com.Id = u.CompanyId");
            var clause = " WHERE ";

            if (!string.IsNullOrWhiteSpace(jobSeekerID))
            {
                query += clause += "c.JobSeekerID = @JobseekerId";
                clause = " AND ";
            }
            if (!string.IsNullOrWhiteSpace(surname))
            {
                query += clause += "c.Surname = @Surname";
                clause = " AND ";
            }
            if (!string.IsNullOrWhiteSpace(username))
            {
                query += clause += "u.Email = @Username";
                clause = " AND ";
            }
            if (!string.IsNullOrWhiteSpace(company))
            {
                query += clause += "com.Name = @Company";
                clause = " AND ";
            }
            if (dateFrom.HasValue)
            {
                query += clause += "c.CreatedDate > @DateFrom";
                clause = " AND ";
            }
            if (dateTo.HasValue)
            {
                query += clause += "c.CreatedDate < @DateTo";
                clause = " AND ";
            }
            query += " ORDER BY c.CreatedDate DESC";

            var model = new ClientQueryViewModel {JobSeekerID = jobSeekerID, Surname = surname, DateFrom = dateFrom, DateTo = dateTo, Username = username, Company = company };
            
            return Query<ClientResultViewModel>(query, model).AsQueryable();
        }

        public IList<ClientCsvModel> GetCsvData(DateTime? dateFrom, DateTime? dateTo, string jobseekerId, string surname, string username, string company, string fieldstring)
        {
                var query = string.Format(@"select
                                            c.CreatedDate,
                                            com.Name AS Company,
                                            u.Email AS Username,
                                            c.HowManyTimesHasSurveyBeenCompleted,
                                            a.ActionType AS ActionName,
                                            cr.AnswerString,
                                            cr.PreContemplationPoints,
                                            cr.MatrixPreContemplationPoints,
                                            cr.ContemplationPoints,
                                            cr.MatrixContemplationPoints,
                                            cr.ActionPoints,
                                            cr.MatrixActionPoints,
                                            s.Name AS SchemeName,");
                query += fieldstring;
                query += string.Format(@" FROM [dbo].[ClientNew] c 
                                            INNER JOIN [dbo].[ClientResult] cr ON cr.ClientId = c.Id
                                            INNER JOIN [dbo].[ActionType] a ON a.Id = cr.ActionIdToDisplay
                                            INNER JOIN [dbo].[User] u ON c.UserId = u.Id
                                            INNER JOIN [dbo].[CompanyNew] com ON com.Id = u.CompanyId
                                            INNER JOIN [dbo].[Scheme] s ON c.SchemeId = s.Id");
            var clause = " WHERE ";
            
            if (!string.IsNullOrWhiteSpace(jobseekerId))
            {
                query += clause += "c.JobSeekerID = @JobseekerId";
                clause = " AND ";
            }
            if (!string.IsNullOrWhiteSpace(surname))
            {
                query += clause += "c.Surname = @Surname";
                clause = " AND ";
            }
            if (!string.IsNullOrWhiteSpace(username))
            {
                query += clause += "u.Email = @Username";
                clause = " AND ";
            }
            if (!string.IsNullOrWhiteSpace(company))
            {
                query += clause += "com.Name = @Company";
                clause = " AND ";
            }
            if (dateFrom.HasValue)
            {
                query += clause += "c.CreatedDate > @DateFrom";
                clause = " AND ";
            }
            if (dateTo.HasValue)
            {
                query += clause += "c.CreatedDate < @DateTo";
                clause = " AND ";
            }
            query += " ORDER BY c.CreatedDate DESC";

            var model = new ClientCsvModel { JobSeekerID = jobseekerId, Surname = surname, DateFrom = dateFrom, DateTo = dateTo, Username = username, Company = company };

            return Query<ClientCsvModel>(query, model).ToList();
        }

        public int GetNumberOfPreviousAttempts(string reference)
        {
            var query = string.Format(@"select count(id) from [dbo].[ClientNew] where JobSeekerId = @jobSeekerID");
            return Query<int>(query, new { JobSeekerID = reference }).FirstOrDefault();
        }
    }

    public interface IClientRepository : IRepository<ClientViewModel>
    {
        int InsertPerson(ClientViewModel client);
        int InsertResult(ResultViewModel result);
        ClientViewModel GetClient(int id);
        IQueryable<ClientResultViewModel> All(DateTime? dateFrom, 
        DateTime? dateTo, string jobseekerId, string surname, string username, string company);
        IList<ClientCsvModel> GetCsvData(DateTime? dateFrom, DateTime? dateTo, string jobseekerId, string surname, string username, string company, string fieldstring);
        int GetNumberOfPreviousAttempts(string reference);

    }
}
