using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using A4EPARC.Enums;
using A4EPARC.ViewModels;

namespace A4EPARC.Repositories
{
    public class SiteTextRepository : Repository<SiteTextViewModel>, ISiteTextRepository
    {
        public List<SiteTextViewModel> Get(SiteTextCode code, int? schemeId, string languageCode)
        {
            var sitetext = HttpContext.Current.Cache["GetSiteText" + code] as List<SiteTextViewModel>;

            if (sitetext  != null)
            {
                if (sitetext.Any())
                {
                    return sitetext.Where(s => s.Code == (int)code && s.SchemeId == schemeId.GetValueOrDefault() && s.LanguageCode == languageCode).ToList();
                }
            }

            const string query = @"SELECT Id, Code, SchemeId, LanguageCode, Description, Name, Summary, [OrderNumber] FROM [dbo].[SiteText] ORDER BY OrderNumber";
            sitetext = (List<SiteTextViewModel>)Query<SiteTextViewModel>(query);

            HttpContext.Current.Cache.Insert("GetSiteText" + code, sitetext, null, DateTime.Now.AddHours(2), TimeSpan.Zero);

            return sitetext.Where(s => s.Code == (int)code && s.SchemeId == schemeId.GetValueOrDefault() && s.LanguageCode == languageCode).ToList();
        }

        public List<SiteTextViewModel> GetJtableView()
        {
            return
                Query<SiteTextViewModel>(
                    @"SELECT s.Id, s.Code, s.LanguageCode, s.Name, s.[Description], s.Summary, s.SchemeId, s.[OrderNumber] FROM [dbo].[SiteText] s")
                    .ToList();
        }

        public int Add(SiteTextViewModel model)
        {
            return
                Execute(
                    @"INSERT INTO [dbo].[SiteText] (Code, LanguageCode, [Description], [Name], Summary, SchemeId, [OrderNumber]) VALUES (@Code, @LanguageCode, @Description, @Name, @Summary, @SchemeId, @OrderNumber);SELECT CAST(SCOPE_IDENTITY() as int)", model);
        }

        public int Save(SiteTextViewModel model)
        {
            return
                Execute(
                    @"UPDATE [dbo].[SiteText] SET LanguageCode = @LanguageCode, Code = @Code, Description= @Description, [Name] = @Name, [Summary] = @Summary, SchemeId = @SchemeId, @OrderNumber = OrderNumber WHERE Id = @Id", model);
        }
    }

    public interface ISiteTextRepository : IRepository<SiteTextViewModel>
    {
        List<SiteTextViewModel> Get(SiteTextCode code, int? schemeId, string languageCode);
        List<SiteTextViewModel> GetJtableView();
        int Add(SiteTextViewModel model);
        int Save(SiteTextViewModel model);
    }
}