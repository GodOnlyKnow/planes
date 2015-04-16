using System.Web;
using System.Web.Optimization;

namespace Planes
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/jasny-bootstrap.min.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Buttons.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/jasny-bootstrap.min.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/sbadmin").Include(
                "~/Content/vendor/bootstrap/dist/css/bootstrap.min.css",
                "~/Content/vendor/metisMenu/dist/metisMenu.min.css",
                "~/Content/vendor/sbadmin/css/timeline.css",
                "~/Content/vendor/sbadmin/css/sb-admin-2.css",
                "~/Content/vendor/font-awesome/css/font-awesome.min.css",
                "~/Content/jasny-bootstrap.min.css",
                "~/Content/Buttons.css",
                "~/Content/magicsuggest/magicsuggest-min.css",
                "~/Content/fileupload/fileinput.min.css"
                ));
            bundles.Add(new ScriptBundle("~/bundles/sbadmin").Include(
                "~/Content/vendor/jquery/dist/jquery.min.js",
                "~/Content/vendor/bootstrap/dist/js/bootstrap.min.js",
                "~/Content/vendor/metisMenu/dist/metisMenu.min.js",
                "~/Scripts/jasny-bootstrap.min.js",
                "~/Content/vendor/sbadmin/js/sb-admin-2.js",
                "~/Content/magicsuggest/magicsuggest-min.js",
                "~/Content/fileupload/fileinput.min.js",
                "~/Content/fileupload/fileinput_locale_zh.js"
                ));
        }
    }
}
