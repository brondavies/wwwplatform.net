using System.Web;
using System.Web.Optimization;
using wwwplatform.Models;
using wwwplatform.Models.Support;

namespace wwwplatform
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            var Context = new HttpContextWrapper(HttpContext.Current);
            var settings = Settings.Create(Context);

            bundles.Add(new ScriptBundle("~/Scripts/jquery").Include("~/Scripts/jquery-{version}.js"));

            string skin = settings.SkinDefinitionFile ??  "~/App_Data/Skins/Default/skin.json";
            SkinDefinition skindef = SkinDefinition.Load(Context.Server.MapPath(skin));
            foreach (var script in skindef.scripts.Keys)
            {
                bundles.Add(new ScriptBundle(script).Include(skindef.scripts[script].ToArray()));
            }

            foreach (var css in skindef.css.Keys)
            {
                StyleBundle bundle = new StyleBundle(css);
                foreach(string file in skindef.css[css])
                {
                    bundle.Include(file, new CssRewriteUrlTransform());
                }
                bundles.Add(bundle);
            }
        }
    }
}
