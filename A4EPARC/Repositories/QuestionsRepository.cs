using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Caching;
using A4EPARC.ViewModels;

namespace A4EPARC.Repositories
{
    public class QuestionsRepository : Repository<QuestionViewModel>, IQuestionsRepository
    {
        public List<QuestionViewModel> Get(int? schemeId, string languageCode)
        {
            var questionsList = HttpContext.Current.Cache["GetQuestions"] as List<QuestionViewModel>;

            if (questionsList != null)
            {
                if (questionsList.Any())
                {
                    foreach (var question in questionsList)
                    {
                        question.Answer = null;
                    }
                    return questionsList.Where(q => q.SchemeId == schemeId.GetValueOrDefault() && q.LanguageCode == languageCode).ToList();
                }
            }

            const string query = @"SELECT Id, Code, Description,ActionTypeId, SchemeId, LanguageCode, Code AS OrderNumber FROM [dbo].[Question]";
            questionsList = (List<QuestionViewModel>)Query<QuestionViewModel>(query);

            HttpContext.Current.Cache.Insert("GetQuestions", questionsList, null, DateTime.Now.AddHours(2), TimeSpan.Zero);

            return questionsList.Where(q => q.SchemeId == schemeId.GetValueOrDefault() && q.LanguageCode == languageCode).ToList();
        }

        public List<QuestionViewModel> GetJtableView()
        {
            return
                Query<QuestionViewModel>(
                    @"SELECT q.Id, q.Code, q.ActionTypeId, q.LanguageCode, q.[Description], q.SchemeId, q.Code AS OrderNumber FROM [dbo].[Question] q")
                    .ToList();
        }

        public int Save(QuestionViewModel model)
        {
            return
                Execute(
                    @"UPDATE [dbo].[Question] SET Description= @Description WHERE Id = @Id", model);
        }
    }

    public interface IQuestionsRepository : IRepository<QuestionViewModel>
    {
        List<QuestionViewModel> Get(int? schemeId, string languageCode);
        int Save(QuestionViewModel model);
        List<QuestionViewModel> GetJtableView();
    }
}