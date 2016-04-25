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
            "~/Scripts/respond.*",
            "~/Scripts/jquery.isotope.js",
            "~/Scripts/jquery.slicknav.js",
            "~/Scripts/jquery.visible.js",
            "~/Scripts/slimbox2.js",
            "~/Scripts/tinymce/jquery.tinymce.js",
            "~/Scripts/scripts.js",
            "~/Scripts/custom.js"
            ));

            bundles.Add(new ScriptBundle("~/scripts/modernizr").Include(
                "~/Scripts/modernizr.custom.js"
                ));

            bundles.Add(new StyleBundle("~/assets/css").Include(
                "~/assets/css/animate.css",
                "~/assets/css/nexus.css",
                "~/assets/css/responsive.css",
                "~/assets/css/custom.css"));

        }
    }
}
