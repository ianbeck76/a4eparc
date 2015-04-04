using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using A4EPARC.Enums;
using A4EPARC.ViewModels;

namespace A4EPARC.Repositories
{
    public class SiteLabelsRepository : Repository<SiteLabelsViewModel>, ISiteLabelsRepository
    {
        public List<SiteLabelsViewModel> Get(int? schemeId, string languageCode)
        {
            var siteLabels = HttpContext.Current.Cache["GetSiteLabels" + schemeId.GetValueOrDefault() + languageCode] as List<SiteLabelsViewModel>;

            if (siteLabels  != null)
            {
                if (siteLabels.Any())
                {
                    return siteLabels;
                }
            }

            var query = @"SELECT DISTINCT e.Name, 
                                    COALESCE(s.Description,e.Description) AS Description, 
                                    COALESCE(s.SchemeId, e.SchemeId) AS SchemeId, 
                                    COALESCE(s.LanguageCode, e.LanguageCode) AS LanguageCode
                                    FROM [dbo].[SiteLabels] e
                                    LEFT JOIN [dbo].[SiteLabels] s
                                    ON s.Name = e.Name 
                                    AND s.LanguageCode = @LanguageCode 
                                    AND s.SchemeId = @SchemeId
                                    WHERE e.SchemeId = 1 
                                    AND e.LanguageCode = 'en-GB' 
                                    ORDER BY e.Name";
            siteLabels = (List<SiteLabelsViewModel>)Query<SiteLabelsViewModel>(query, new { LanguageCode = languageCode, SchemeId = schemeId.GetValueOrDefault() }).ToList();

            HttpContext.Current.Cache.Insert("GetSiteLabels" + schemeId.GetValueOrDefault() + languageCode, siteLabels, null, DateTime.Now.AddHours(2), TimeSpan.Zero);

            return siteLabels;
        }

        public List<SiteLabelsViewModel> All()
        {
            var siteLabels = HttpContext.Current.Cache["GetSiteLabels"] as List<SiteLabelsViewModel>;

            if (siteLabels != null)
            {
                if (siteLabels.Any())
                {
                    return siteLabels;
                }
            }

            var query = @"SELECT s.Name, s.Description,s.SchemeId,s.LanguageCode
                                    FROM [dbo].[SiteLabels] s
                                    ORDER BY s.Name";
            siteLabels = (List<SiteLabelsViewModel>)Query<SiteLabelsViewModel>(query).ToList();

            HttpContext.Current.Cache.Insert("GetSiteLabels", siteLabels, null, DateTime.Now.AddHours(24), TimeSpan.Zero);

            return siteLabels;
        }


        public List<SiteLabelsViewModel> GetJtableView()
        {
            return
                Query<SiteLabelsViewModel>(
                    @"SELECT s.Id, s.LanguageCode, s.Name, s.[Description], s.SchemeId FROM [dbo].[SiteLabels] s")
                    .ToList();
        }

        public int Add(SiteLabelsViewModel model)
        {
            return
                Execute(
                    @"INSERT INTO [dbo].[SiteLabels] (LanguageCode, [Description], [Name], SchemeId) VALUES (@LanguageCode, @Description, @Name, @SchemeId);SELECT CAST(SCOPE_IDENTITY() as int)", model);
        }

        public int Save(SiteLabelsViewModel model)
        {
            return
                Execute(
                    @"UPDATE [dbo].[SiteLabels] SET LanguageCode = @LanguageCode, Description= @Description, [Name] = @Name, SchemeId = @SchemeId WHERE Id = @Id", model);
        }
    }

    public interface ISiteLabelsRepository : IRepository<SiteLabelsViewModel>
    {
        List<SiteLabelsViewModel> Get(int? schemeId, string languageCode);
        List<SiteLabelsViewModel> GetJtableView();
        int Add(SiteLabelsViewModel model);
        int Save(SiteLabelsViewModel model);
        List<SiteLabelsViewModel> All();
    }
}