using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wwwplatform.Shared.Extensions.System;

namespace wwwplatform.Models
{
    public class Settings
    {
        public static string AppSetting(string key, string defaultValue = null)
        {
            return ConfigurationManager.AppSettings[key] ?? defaultValue;
        }

        public static bool AllowForgotPassword { get { return Boolean.Parse(AppSetting("AllowForgotPassword", Boolean.FalseString)); } }

        public static bool AllowUserRegistration { get { return Boolean.Parse(AppSetting("AllowUserRegistration", Boolean.FalseString)); } }

        public static string CanonicalHostName { get { return AppSetting("CanonicalHostName"); } }

        public static string DefaultPageDescription { get { return AppSetting("DefaultPageDescription"); } }

        public static string DefaultPageTitle { get { return AppSetting("DefaultPageTitle"); } }

        public static string DefaultSiteImage { get { return AppSetting("DefaultSiteImage"); } }

        public static string EmailDefaultFrom { get { return AppSetting("EmailDefaultFrom"); } }

        public static string SiteName { get { return AppSetting("SiteName"); } }

        public static string SiteOwner { get { return AppSetting("SiteOwner"); } }

        public static string TempDir { get { return AppSetting("TempDir") ?? "~/App_Data/temp".ResolveLocalPath(); } }

        public static string UserFilesDir { get { return AppSetting("UserFilesDir") ?? "~/UserFiles".ResolveLocalPath(); } }
    }
}
