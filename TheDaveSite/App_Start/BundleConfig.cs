using System.Web;
using System.Web.Optimization;

namespace TheDaveSite
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/plugins").Include(
                "~/Scripts/Plugins/WaitForImages/waitForImages.js"));

            bundles.Add(new ScriptBundle("~/bundles/decorations_script").Include(
                "~/Scripts/DaveScript/Decorations/DoorShutter.js",
                "~/Scripts/DaveScript/Decorations/Popper.js",
                "~/Scripts/DaveScript/Decorations/Expando.js",
                "~/Scripts/DaveScript/Decorations/Griddlizer.js"));

            bundles.Add(new ScriptBundle("~/bundles/controls_script").Include(
                "~/Scripts/DaveScript/Controls/LinkDisplayer.js"));

            bundles.Add(new ScriptBundle("~/bundles/DaveScript").Include(
                "~/Scripts/DaveScript/Spinner.js",
                "~/Scripts/DaveScript/GlobalScript.js",
                "~/Scripts/DaveScript/LayoutScript.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/bundles/decorations_styles").Include(
                "~/Content/Decorations/DoorShutter.css",
                "~/Content/Decorations/Popper.css",
                "~/Content/Decorations/Expando.css"));

            bundles.Add(new StyleBundle("~/bundles/controls_styles").Include(
                "~/Content/Controls/LinkDisplayer.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

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
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new LessBundle("~/bundles/less").Include(
                "~/Content/Less/base.less",
                "~/Content/Less/site.less"));

            bundles.Add(new LessBundle("~/Content/Pages/Blog")
                .Include("~/Content/Less/Pages/Blog/Blog.less"));
        }
    }
}