using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using A4EPARC.Repositories;
using A4EPARC.Services;
using A4EPARC.ViewModels;

namespace A4EPARC.App_Start
{
    public static class CacheConfig
    {
        public static void Initialise()
        {
            var questionRepository = DependencyResolver.Current.GetService<IQuestionRepository>();
            const string query = @"SELECT Id, Id AS Code, Description,ActionTypeId, SchemeId, LanguageCode FROM [dbo].[Question]";
            var questionsList = (List<QuestionViewModel>) questionRepository.Query(query);
            HttpContext.Current.Cache.Insert("GetQuestions", questionsList, null, DateTime.Now.AddHours(2), TimeSpan.Zero);

            var siteTextRepository = DependencyResolver.Current.GetService<ISiteTextRepository>();
            const string siteText = @"SELECT Id, Code, SchemeId, LanguageCode, Description, Name, Summary FROM [dbo].[SiteText]";
            var siteTextList = (List<SiteTextViewModel>)siteTextRepository.Query(siteText);
            HttpContext.Current.Cache.Insert("GetSiteText", siteTextList, null, DateTime.Now.AddHours(2), TimeSpan.Zero);
        }        
    }
}