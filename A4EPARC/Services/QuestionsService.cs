using A4EPARC.Enums;
using A4EPARC.Repositories;
using A4EPARC.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace A4EPARC.Services
{
 
    public class QuestionsService : IQuestionsService
    {
        private IQuestionsRepository QuestionsRepository;

        public QuestionsService(IQuestionsRepository questionRepository) { QuestionsRepository = questionRepository; }

        public List<QuestionViewModel> Get(int schemeId, string languageCode)
        {
            var questions = QuestionsRepository.Get(schemeId, languageCode);

            if (!questions.Any())
            {
                questions = QuestionsRepository.Get(schemeId, "en-GB");

                if (!questions.Any())
                {
                    questions = QuestionsRepository.Get(1, "en-GB");
                }
            }
            return questions.OrderBy(q => q.OrderNumber).ToList();
        } 
    }

    public interface IQuestionsService 
    {
        List<QuestionViewModel> Get(int schemeId, string languageCode);
    }
}