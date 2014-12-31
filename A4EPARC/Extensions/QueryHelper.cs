using System.Linq;
using System.Web.Mvc;

namespace A4EPARC.Extensions
{
    public static class QueryHelper
    {
        public static SelectList ToSelectList<T>(this IQueryable<T> query, string dataValueField, string dataTextField, object selectedValue)
        {
            return new SelectList(query, dataValueField, dataTextField, selectedValue ?? -1);
        }
    }
}