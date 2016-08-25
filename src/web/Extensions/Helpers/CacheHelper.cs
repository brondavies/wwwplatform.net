using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace wwwplatform.Extensions.Helpers
{
    internal class CacheHelper
    {
        internal static T GetFromCacheOrDefault<T>(HttpContextBase HttpContext, Action<T> constructor = null)
        {
            Type type = typeof(T);
            return GetFromCacheOrDefault<T>(HttpContext, type.FullName, type, constructor);
        }

        internal static T GetFromCacheOrDefault<T>(HttpContextBase HttpContext, Type type, Action<T> constructor = null)
        {
            return GetFromCacheOrDefault<T>(HttpContext, type.FullName, type, constructor);
        }

        internal static T GetFromCacheOrDefault<T>(HttpContextBase HttpContext, string key, Action<T> constructor = null)
        {
            return GetFromCacheOrDefault<T>(HttpContext, key, typeof(T), constructor);
        }

        private static T GetFromCacheOrDefault<T>(HttpContextBase HttpContext, string key, Type type, Action<T> constructor = null)
        {
            T result = default(T);
            try
            {
                result = (T)HttpContext.Cache[key];
            }
            catch { }
            if (result == null)
            {
                result = (T)Activator.CreateInstance(type, true);
                constructor?.Invoke(result);
                try
                {
                    HttpContext.Cache.Add(key, result, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                }
                catch { }
            }
            return result;
        }

        internal static void ClearFromCache(HttpContextBase HttpContext, Type type)
        {
            string key = type.FullName;
            ClearFromCache(HttpContext, key);
        }

        internal static void ClearFromCache(HttpContextBase HttpContext, string key)
        {
            HttpContext.Cache.Remove(key);
        }
    }
}
