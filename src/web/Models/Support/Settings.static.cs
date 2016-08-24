using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using wwwplatform.Extensions.Helpers;
using wwwplatform.Shared.Extensions.System;

namespace wwwplatform.Models
{
    public partial class Settings : ObjectDictionary
    {
        public static string AppSetting(string key, string defaultValue = null)
        {
            return ConfigurationManager.AppSettings[key] ?? defaultValue;
        }

        static bool DefaultAllowForgotPassword { get { return Boolean.Parse(AppSetting("AllowForgotPassword", Boolean.FalseString)); } }

        static bool DefaultAllowUserRegistration { get { return Boolean.Parse(AppSetting("AllowUserRegistration", Boolean.FalseString)); } }

        static string DefaultCanonicalHostName { get { return AppSetting("CanonicalHostName"); } }

        static string DefaultEmailFrom { get { return AppSetting("EmailDefaultFrom"); } }

        static string DefaultSiteName { get { return AppSetting("SiteName"); } }

        static string DefaultSiteOwner { get { return AppSetting("SiteOwner"); } }

        static string DefaultTempDir { get { return AppSetting("TempDir") ?? "~/App_Data/temp".ResolveLocalPath(); } }

        static string DefaultUserFilesDir { get { return AppSetting("UserFilesDir") ?? "~/UserFiles".ResolveLocalPath(); } }

        public static Settings Create(HttpContextBase Context)
        {
            return CacheHelper.GetFromCacheOrDefault<Settings>(Context, typeof(Settings), (value) => {
                var db = Context.GetOwinContext().Get<ApplicationDbContext>();
                var list = db.AppSettings.ToList();
                foreach (var item in list)
                {
                    value[item.Name] = item.Value;
                }
            });
        }
    }
}
