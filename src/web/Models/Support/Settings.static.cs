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

        static string DefaultAllowForgotPassword { get { return AppSetting("AllowForgotPassword", bool.FalseString); } }

        static string DefaultAllowUserRegistration { get { return AppSetting("AllowUserRegistration", bool.FalseString); } }

        static string DefaultCanonicalHostName { get { return AppSetting("CanonicalHostName"); } }

        static string DefaultEmailFrom { get { return AppSetting("EmailDefaultFrom"); } }

        static string DefaultShowSharedFoldersInMenus { get { return AppSetting("ShowSharedFoldersInMenus", bool.TrueString); } }

        static string DefaultSiteName { get { return AppSetting("SiteName"); } }

        static string DefaultSiteOwner { get { return AppSetting("SiteOwner"); } }

        static string DefaultTempDir { get { return AppSetting("TempDir") ?? "~/App_Data/temp".ResolveLocalPath(); } }

        static string DefaultUserFilesDir { get { return AppSetting("UserFilesDir") ?? "~/UserFiles".ResolveLocalPath(); } }

        public static Settings Create(HttpContextBase Context)
        {
            return CacheHelper.GetFromCacheOrDefault<Settings>(Context, (value) => {
                try
                {
                    var db = Context.GetOwinContext().Get<ApplicationDbContext>();
                    var list = db.AppSettings.ToList();
                    foreach (var item in list)
                    {
                        value[item.Name] = item.Value;
                    }
                }
                catch { }
            });
        }
    }
}
