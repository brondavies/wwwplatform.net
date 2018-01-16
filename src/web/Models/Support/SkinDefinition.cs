using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using wwwplatform.Extensions.Helpers;

namespace wwwplatform.Models.Support
{
    public class SkinDefinition
    {
        public static string LayoutCacheKey { get { return "SkinDefLayout"; } }

        public Dictionary<string, List<string>> scripts { get; set; }
        public Dictionary<string, List<string>> css { get; set; }
        public string layout { get; set; }

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
