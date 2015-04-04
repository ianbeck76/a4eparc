using System.Web;
using System.Web.Optimization;

namespace A4EPARC
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-1.10.2.js",
                "~/Scripts/jquery-migrate-1.2.1.js",
                "~/Scripts/jquery.cookie.js",
                "~/Scripts/ui-validation.js",
                "~/Scripts/reportFilters.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-1.9.2.js",
                "~/Scripts/jquery-ui-1.10.4.custom.js"));

            bundles.Add(new ScriptBundle("~/bundles/ie8specific").Include(
            "~/Scripts/html5shiv.js",
            "~/Scripts/respond.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                      "~/Scripts/jquery.validate.js",
                      "~/Scripts/jquery.validate.unobtrusive.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Content/bootstrap/js/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/ui").Include(
                "~/Scripts/bootstrap-datepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/jtable").IncludeDirectory(
                "~/Scripts/jtable", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/css/theme.css"));

            bundles.Add(new StyleBundle("~/Content/css_gls").Include("~/Content/css_gls/theme.css"));

            bundles.Add(new StyleBundle("~/Content/css_a4e").Include("~/Content/css_a4e/theme.css"));
   
            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                "~/Content/themes/base/jquery.ui.core.css",
                "~/Content/themes/base/jquery.ui.resizable.css",
                "~/Content/themes/base/jquery.ui.selectable.css",
                "~/Content/themes/base/jquery.ui.accordion.css",
                "~/Content/themes/base/jquery.ui.autocomplete.css",
                "~/Content/themes/base/jquery.ui.button.css",
                "~/Content/themes/base/jquery.ui.dialog.css",
                "~/Content/themes/base/jquery.ui.slider.css",
                "~/Content/themes/base/jquery.ui.tabs.css",
                "~/Content/themes/base/jquery.jqplot.css",
                "~/Content/themes/base/jquery.ui.progressbar.css",
                "~/Content/themes/base/jquery.ui.theme.css",
                "~/Content/jquery.timepicker.css"));
        }
    }
}