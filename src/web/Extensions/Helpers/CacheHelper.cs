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
        internal static T GetFromCacheOrDefault<T>(HttpContextBase HttpContext, Type type, Action<T> constructor = null)
        {
            string key = type.GetType().Name;
            T result = (T)HttpContext.Cache[key];
            if (result == null)
            {
                result = (T)Activator.CreateInstance(type, true);
                constructor?.Invoke(result);
                HttpContext.Cache.Add(key, result, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }
            return result;
        }

        internal static T GetFromCacheOrDefault<T>(HttpContextBase HttpContext,  string key, Action<T> constructor = null)
        {
            T result = (T)HttpContext.Cache[key];
            if (result == null)
            {
                result = (T)Activator.CreateInstance(typeof(T), true);
                constructor?.Invoke(result);
                HttpContext.Cache.Add(key, result, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }
            return result;
        }
    }
}
