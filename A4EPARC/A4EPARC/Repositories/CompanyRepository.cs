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
        public List<CompanySchemeViewModel> GetSchemes()
        {
            var schemeList = HttpContext.Current.Cache["GetSchemes"] as List<CompanySchemeViewModel>;

            if (schemeList != null)
            {
                return schemeList;
            }

            const string query = @"SELECT cs.CompanyId, cs.SchemeId, s.Name as SchemeName FROM [dbo].[CompanyScheme] cs INNER JOIN [dbo].[Scheme] s ON s.Id = cs.SchemeId";
            schemeList = (List<CompanySchemeViewModel>)Query<CompanySchemeViewModel>(query).ToList();

            HttpContext.Current.Cache.Insert("GetSchemes", schemeList, null, DateTime.Now.AddHours(2), TimeSpan.Zero);

            return schemeList;
        }
    }

    public interface ICompanyRepository : IRepository<Company>
    {
        List<CompanySchemeViewModel> GetSchemes();
    }
}