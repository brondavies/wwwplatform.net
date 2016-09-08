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
            var settings = Settings.Create(new HttpContextWrapper(HttpContext.Current));

            string skin = settings.SkinDefinitionFile ??  "~/App_Data/Skins/Default/skin.json";
            SkinDefinition skindef = SkinDefinition.Load(HttpContext.Current.Server.MapPath(skin));
            HttpContext.Current.Application["Layout"] = skindef.layout;
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
