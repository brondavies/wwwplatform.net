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
        private const string kAllowForgotPassword = "AllowForgotPassword";
        private const string kAllowUserRegistration = "AllowUserRegistration";
        private const string kCanonicalHostName = "CanonicalHostName";
        private const string kDefaultPageDescription = "DefaultPageDescription";
        private const string kDefaultPageLayout = "DefaultPageLayout";
        private const string kDefaultPageTitle = "DefaultPageTitle";
        private const string kDefaultSiteImage = "DefaultSiteImage";
        private const string kDefaultUploadPermissions = "DefaultUploadPermissions";
        private const string kEmailDefaultFrom = "EmailDefaultFrom";
        private const string kShowSharedFoldersInMenus = "ShowSharedFoldersInMenus";
        private const string kSiteName = "SiteName";
        private const string kSiteOwner = "SiteOwner";
        private const string kSkinDefinitionFile = "SkinDefinitionFile";
        private const string kTempDir = "TempDir";
        private const string kUserFilesDir = "UserFilesDir";

        public static string GetConfig(string key, string defaultValue = null)
        {
            return ConfigurationManager.AppSettings[key] ?? defaultValue;
        }

        static string DefaultAllowForgotPassword { get { return GetConfig(kAllowForgotPassword, bool.FalseString); } }

        static string DefaultAllowUserRegistration { get { return GetConfig(kAllowUserRegistration, bool.FalseString); } }

        static string DefaultShowSharedFoldersInMenus { get { return GetConfig(kShowSharedFoldersInMenus, bool.TrueString); } }

        static string DefaultTempDir { get { return GetConfig(kTempDir) ?? "~/App_Data/temp".ResolveLocalPath(); } }

        static string DefaultUserFilesDir { get { return GetConfig(kUserFilesDir) ?? "~/UserFiles".ResolveLocalPath(); } }

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
                    if (string.IsNullOrEmpty(value.SkinDefinitionFile))
                    {
                        value[kSkinDefinitionFile] = Context.Application["Layout"];
                    }
                }
                catch { }
            });
        }

        internal static AppSetting[] GetDefaultSettings()
        {
            return new [] {
                new AppSetting { Name = kAllowForgotPassword, Kind = AppSetting.KindBool, DefaultValue = DefaultAllowForgotPassword, Description = "Allows users to reset forgotten passwords" },
                new AppSetting { Name = kAllowUserRegistration, Kind = AppSetting.KindBool, DefaultValue = DefaultAllowUserRegistration, Description = "Opens user registration to the public" },
                new AppSetting { Name = kCanonicalHostName, Kind = AppSetting.KindString, Description = "The host name that should be used for this site" },
                new AppSetting { Name = kDefaultPageDescription, Kind = AppSetting.KindString, Description = "The default page description" },
                new AppSetting { Name = kDefaultPageLayout, Kind = AppSetting.KindFile, Description = "The default page layout file" },
                new AppSetting { Name = kDefaultPageTitle, Kind = AppSetting.KindString, Description = "The default page title" },
                new AppSetting { Name = kDefaultSiteImage, Kind = AppSetting.KindUpload, Description = "The default image used for links shared via social media" },
                new AppSetting { Name = kSkinDefinitionFile, Kind = AppSetting.KindFile, Description = "The site skin definition file" },
                new AppSetting { Name = kDefaultUploadPermissions, Kind = AppSetting.KindRole, Description = "The default permissions for files uploaded by any user" },
                new AppSetting { Name = kEmailDefaultFrom, Kind = AppSetting.KindString, Description = "The email address used for emails sent by the system" },
                new AppSetting { Name = kShowSharedFoldersInMenus, Kind = AppSetting.KindBool, DefaultValue = DefaultShowSharedFoldersInMenus, Description = "Add a link to shared folders in the navigation and footer. Shared Folder links are always available to the roles assigned." },
                new AppSetting { Name = kSiteName, Kind = AppSetting.KindString, Description = "The name of the site" },
                new AppSetting { Name = kSiteOwner, Kind = AppSetting.KindString, Description = "The name of the site owner" },
                new AppSetting { Name = kTempDir, Kind = AppSetting.KindDirectory, Description = "The directory used for temporary files" },
                new AppSetting { Name = kUserFilesDir, Kind = AppSetting.KindDirectory, Description = "The directory used for files uploaded by users" }
            };
        }
    }
}
