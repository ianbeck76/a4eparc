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
            var questionRepository = DependencyResolver.Current.GetService<IQuestionsRepository>();
            const string query = @"SELECT Id, Code, Description,ActionTypeId, SchemeId, LanguageCode, Code AS OrderNumber FROM [dbo].[Question]";
            var questionsList = (List<QuestionViewModel>) questionRepository.Query(query);
            HttpContext.Current.Cache.Insert("GetQuestions", questionsList, null, DateTime.Now.AddHours(2), TimeSpan.Zero);

            var siteLabelsRepository = DependencyResolver.Current.GetService<ISiteLabelsRepository>();
            const string siteLabels = @"SELECT Id, SchemeId, LanguageCode, Description, Name FROM [dbo].[SiteLabels]";
            var siteLabelsList = (List<SiteLabelsViewModel>)siteLabelsRepository.Query(siteLabels);
            HttpContext.Current.Cache.Insert("GetSiteText", siteLabelsList, null, DateTime.Now.AddHours(2), TimeSpan.Zero);
        }        
    }
}