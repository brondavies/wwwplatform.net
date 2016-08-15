using System.Web;
using System.Web.Optimization;

namespace wwwplatform
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts/substance").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery.validate*",
                "~/Scripts/jquery.cookie.js",
                "~/Scripts/respond.*",
                "~/Scripts/jquery.isotope.js",
                "~/Scripts/jquery.slicknav.js",
                "~/Scripts/jquery.visible.js",
                "~/Scripts/slimbox2.js",
                "~/Scripts/scripts.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/bootstrap-switch.js",
                "~/Scripts/jquery.dataTables.js",
                "~/Scripts/dataTables.bootstrap.js",
                "~/Scripts/custom.js"
            ));

            bundles.Add(new ScriptBundle("~/scripts/modernizr").Include(
                "~/Scripts/modernizr.custom.js"
            ));

            bundles.Add(new StyleBundle("~/assets/css").Include(
                "~/assets/css/animate.css",
                "~/assets/css/nexus.css",
                "~/assets/css/responsive.css",
                "~/Content/bootstrap-switch/bootstrap3/bootstrap-switch.css",
                "~/Content/jquery.dataTables.css",
                "~/Content/dataTables.bootstrap.css",
                "~/assets/css/custom.css"
            ));

        }
    }
}
