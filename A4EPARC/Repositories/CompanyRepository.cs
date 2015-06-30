using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using A4EPARC.Models;
using A4EPARC.ViewModels;

namespace A4EPARC.Repositories
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public Company SingleOrDefault(int id)
        {
            return Query<Company>("SELECT * FROM [dbo].[CompanyNew] WHERE Id = @Id", new { Id = id }).SingleOrDefault();
        }

        public List<CompanySchemeViewModel> GetSchemes()
        {
            var schemeList = HttpContext.Current.Cache["GetSchemes"] as List<CompanySchemeViewModel>;

            if (schemeList != null)
            {
                return schemeList;
            }

            const string query = @"SELECT cs.CompanyId, cs.SchemeId, s.Name as SchemeName FROM [dbo].[CompanyScheme] cs INNER JOIN [dbo].[Scheme] s ON s.Id = cs.SchemeId";
            schemeList = (List<CompanySchemeViewModel>)Query<CompanySchemeViewModel>(query).ToList();

            HttpContext.Current.Cache.Insert("GetSchemes", schemeList, null, DateTime.Now.AddDays(1), TimeSpan.Zero);

            return schemeList;
        }

        public List<CompanyPageItemViewModel> GetPageItems()
        {
            var itemList = HttpContext.Current.Cache["GetPageItems"] as List<CompanyPageItemViewModel>;

            if (itemList != null)
            {
                return itemList;
            }

            const string query = @"SELECT Id, CompanyId, Name, IsDisplay, IsRequired FROM [dbo].[CompanyPageItems]";
            itemList = (List<CompanyPageItemViewModel>)Query<CompanyPageItemViewModel>(query).ToList();

            HttpContext.Current.Cache.Insert("GetPageItems", itemList, null, DateTime.Now.AddDays(1), TimeSpan.Zero);

            return itemList;
        }

        public List<CompanyLanguageViewModel> GetLanguages() 
        {
            var itemList = HttpContext.Current.Cache["GetLanguages"] as List<CompanyLanguageViewModel>;

            if (itemList != null)
            {
                return itemList;
            }

            const string query = @"SELECT Id, CompanyId, Code, Description FROM [dbo].[CompanyLanguages]";
            itemList = (List<CompanyLanguageViewModel>)Query<CompanyLanguageViewModel>(query).ToList();

            HttpContext.Current.Cache.Insert("GetLanguages", itemList, null, DateTime.Now.AddDays(1), TimeSpan.Zero);

            return itemList;
        }

        public List<CompanySelectValuesViewModel> GetSelectValues() 
        {
            var itemList = HttpContext.Current.Cache["CompanySelectValues"] as List<CompanySelectValuesViewModel>;

            if (itemList != null)
            {
                return itemList;
            }

            const string query = @"SELECT [CompanyId],[Key],[Value] FROM [dbo].[CompanySelectValues] ORDER BY [Value]";
            itemList = (List<CompanySelectValuesViewModel>)Query<CompanySelectValuesViewModel>(query).ToList();

            HttpContext.Current.Cache.Insert("CompanySelectValues", itemList, null, DateTime.Now.AddDays(1), TimeSpan.Zero);

            return itemList;
        }

        public Company SingleOrDefault(string where, object parameters)
        {
            return Where(where, parameters).SingleOrDefault();
        }

        public IEnumerable<Company> Where(string where, object parameters)
        {
            return Query<Company>("SELECT * FROM [dbo].[CompanyNew] WHERE " + where, parameters);
        }

        public IEnumerable<Company> All()
        {
            return Query<Company>("SELECT * FROM [dbo].[CompanyNew]");
        }

        public Company Single(int id)
        {
            return Query<Company>("SELECT * FROM [dbo].[CompanyNew] WHERE Id = @Id", new { Id = id }).Single();
        }
    }

    public interface ICompanyRepository : IRepository<Company>
    {
        List<CompanySchemeViewModel> GetSchemes();
        List<CompanyPageItemViewModel> GetPageItems();
        List<CompanyLanguageViewModel> GetLanguages();
        List<CompanySelectValuesViewModel> GetSelectValues();
        Company SingleOrDefault(int id);
        Company SingleOrDefault(string where, object parameters);
        IEnumerable<Company> Where(string where, object parameters);
        IEnumerable<Company> All();
        Company Single(int id);
    }
}