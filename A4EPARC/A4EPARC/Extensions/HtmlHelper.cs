using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace A4EPARC.Extensions
{
    public static class HtmlHelper
    {
        public static MvcHtmlString SocValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
                                                                              Expression<Func<TModel, TProperty>>
                                                                                  expression)
        {
            var midDivBuilder = new TagBuilder("div");
            midDivBuilder.InnerHtml = helper.ValidationMessageFor(expression).ToString().Replace("span","div");
            return MvcHtmlString.Create(midDivBuilder.ToString(TagRenderMode.Normal));
        }
    }
}