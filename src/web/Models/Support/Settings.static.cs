using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
        internal const string kAllowForgotPassword = "AllowForgotPassword";
        internal const string kAllowUserRegistration = "AllowUserRegistration";
        internal const string kCanonicalHostName = "CanonicalHostName";
        internal const string kDefaultPageDescription = "DefaultPageDescription";
        internal const string kDefaultPageLayout = "DefaultPageLayout";
        internal const string kDefaultPageTitle = "DefaultPageTitle";
        internal const string kDefaultSiteImage = "DefaultSiteImage";
        internal const string kDefaultUploadPermissions = "DefaultUploadPermissions";
        internal const string kEmailDefaultFrom = "EmailDefaultFrom";
        internal const string kSharedFoldersLabel = "SharedFoldersLabel";
        internal const string kSharedFoldersRootPermissions = "SharedFoldersRootPermissions";
        internal const string kShowSharedFoldersInMenus = "ShowSharedFoldersInMenus";
        internal const string kSiteName = "SiteName";
        internal const string kSiteOwner = "SiteOwner";
        internal const string kSkinDefinitionFile = "SkinDefinitionFile";
        internal const string kTempDir = "TempDir";
        internal const string kUserFilesDir = "UserFilesDir";

        public static string GetConfig(string key, string defaultValue = null)
        {
            return ConfigurationManager.AppSettings[key] ?? defaultValue;
        }

        static string DefaultAllowForgotPassword { get { return GetConfig(kAllowForgotPassword, bool.FalseString); } }

        static string DefaultAllowUserRegistration { get { return GetConfig(kAllowUserRegistration, bool.FalseString); } }

        static string DefaultShowSharedFoldersInMenus { get { return GetConfig(kShowSharedFoldersInMenus, bool.TrueString); } }

        static string DefaultTempDir { get { return GetConfig(kTempDir) ?? "~/App_Data/temp".ResolveLocalPath(); } }

        static string DefaultUserFilesDir { get { return GetConfig(kUserFilesDir) ?? "~/UserFiles".ResolveLocalPath(); } }

        public static string GetSettingsFileName(HttpContextBase Context)
        {
            return CacheHelper.GetCacheFileName<Settings>(Context);
        }

        public static Settings Create(HttpContextBase Context)
        {
            return CacheHelper.GetFromCacheOrDefault<Settings>(Context, (value) =>
            {
                string settingsFilename = GetSettingsFileName(Context);
                try
                {
                    var db = Context.GetOwinContext().Get<ApplicationDbContext>();
                    var list = db.AppSettings.ToList();
                    foreach (var item in list)
                    {
                        value[item.Name] = item.Value;
                    }
                }
                catch
                {
                    if (File.Exists(settingsFilename))
                    {
                        var dictionary = JsonConvert.DeserializeObject<ObjectDictionary>(File.ReadAllText(settingsFilename));
                        foreach (var entry in dictionary)
                        {
                            value[entry.Key] = entry.Value;
                        }
                    }
                }
                if (!File.Exists(settingsFilename))
                {
                    try
                    {
                        File.WriteAllText(settingsFilename, JsonConvert.SerializeObject(value));
                    }
                    catch { }
                }
            });
        }

        internal static AppSetting[] GetDefaultSettings()
        {
            return new[] {
                new AppSetting { Name = kAllowForgotPassword, Kind = AppSetting.KindBool, DefaultValue = DefaultAllowForgotPassword, Description = "Allows users to reset forgotten passwords" },
                new AppSetting { Name = kAllowUserRegistration, Kind = AppSetting.KindBool, DefaultValue = DefaultAllowUserRegistration, Description = "Opens user registration to the public" },
                new AppSetting { Name = kCanonicalHostName, Kind = AppSetting.KindString, Description = "The host name that should be used for this site" },
                new AppSetting { Name = kDefaultPageDescription, Kind = AppSetting.KindString, Description = "The default page description" },
                new AppSetting { Name = kDefaultPageLayout, Kind = AppSetting.KindFile, Description = "The default page layout file" },
                new AppSetting { Name = kDefaultPageTitle, Kind = AppSetting.KindString, Description = "The default page title" },
                new AppSetting { Name = kDefaultSiteImage, Kind = AppSetting.KindUpload, Description = "The default image used for links shared via social media" },
                new AppSetting { Name = kDefaultUploadPermissions, Kind = AppSetting.KindRole, Description = "The default permissions for files uploaded by any user" },
                new AppSetting { Name = kEmailDefaultFrom, Kind = AppSetting.KindString, Description = "The email address used for emails sent by the system" },
                new AppSetting { Name = kSharedFoldersLabel, Kind = AppSetting.KindString, DefaultValue = "Shared Folders", Description = "The text for the Shared Folders links" },
                new AppSetting { Name = kSharedFoldersRootPermissions, Kind = AppSetting.KindRole, DefaultValue = "", Description = "The roles that can view the root of the shared folders directory. Shared Folder links are always available to the roles assigned." },
                new AppSetting { Name = kShowSharedFoldersInMenus, Kind = AppSetting.KindBool, DefaultValue = DefaultShowSharedFoldersInMenus, Description = "Add a link to Shared Folders in the navigation and footer. Shared Folder links are always available to the roles assigned." },
                new AppSetting { Name = kSiteName, Kind = AppSetting.KindString, Description = "The name of the site" },
                new AppSetting { Name = kSiteOwner, Kind = AppSetting.KindString, Description = "The name of the site owner" },
                new AppSetting { Name = kSkinDefinitionFile, Kind = AppSetting.KindFile, Description = "The site skin definition file" },
                new AppSetting { Name = kTempDir, Kind = AppSetting.KindDirectory, Description = "The directory used for temporary files" },
                new AppSetting { Name = kUserFilesDir, Kind = AppSetting.KindDirectory, Description = "The directory used for files uploaded by users" }
            };
        }
    }
}
