using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Web;
using wwwplatform.Extensions.Helpers;

namespace wwwplatform.Models.Support
{
    public class SkinDefinition
    {
        private Dictionary<string, string> _layouts = new Dictionary<string, string>();

        public static string LayoutCacheKey => "SkinDefLayout";

        public Dictionary<string, List<string>> scripts { get; set; }
        public Dictionary<string, List<string>> css { get; set; }
        public string layout { get; set; }
        public Dictionary<string, string> layouts
        {
            get { return _layouts; }
            set
            {
                if (value == null)
                {
                    _layouts = new Dictionary<string, string>();
                }
                else
                {
                    _layouts = value;
                }
            }
        }

        public static SkinDefinition Load(string skinFile)
        {
            return JsonConvert.DeserializeObject<SkinDefinition>(File.ReadAllText(skinFile));
        }

        public static SkinDefinition Load(HttpContextBase context)
        {
            var settings = Settings.Create(context);
            return Load(context.Server.MapPath(settings.SkinDefinitionFile));
        }

        public static string CurrentLayout(HttpContextBase context)
        {
            string value = CacheHelper.GetFromCacheOrDefault<string>(context, LayoutCacheKey);
            return string.IsNullOrEmpty(value) ? null : value;
        }

        public static void CurrentLayout(HttpContextBase context, string value)
        {
            CacheHelper.PutCache(context, SkinDefinition.LayoutCacheKey, value);
        }
    }
}
