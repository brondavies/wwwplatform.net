using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using wwwplatform.Extensions.Helpers;
using wwwplatform.Shared.Extensions.System;

namespace wwwplatform.Models
{
    public partial class Settings : ObjectDictionary
    {
        internal const string kAllowForgotPassword = "AllowForgotPassword";
        internal const string kAllowUserRegistration = "AllowUserRegistration";
        internal const string kCanonicalHostName = "CanonicalHostName";
        internal const string kConvertPdfExe = "ConvertPdfExe";
        internal const string kCreatePDFVersionsOfDocuments = "CreatePDFVersionsOfDocuments";
        internal const string kDefaultPageDescription = "DefaultPageDescription";
        internal const string kDefaultPageLayout = "DefaultPageLayout";
        internal const string kDefaultPageTitle = "DefaultPageTitle";
        internal const string kDefaultSiteImage = "DefaultSiteImage";
        internal const string kDefaultUploadPermissions = "DefaultUploadPermissions";
        internal const string kEmailDefaultFrom = "EmailDefaultFrom";
        internal const string kMaxEmailSendBatch = "MaxEmailSendBatch";
        internal const string kRolesWithUploadPermission = "RolesWithUploadPermission";
        internal const string kSharedFoldersLabel = "SharedFoldersLabel";
        internal const string kSharedFoldersRootPermissions = "SharedFoldersRootPermissions";
        internal const string kShowSharedFoldersInMenus = "ShowSharedFoldersInMenus";
        internal const string kShowSignInLinks = "ShowSignInLinks";
        internal const string kSiteName = "SiteName";
        internal const string kSiteOwner = "SiteOwner";
        internal const string kSkinDefinitionFile = "SkinDefinitionFile";
        internal const string kTempDir = "TempDir";
        internal const string kThumbnailQuality = "ThumbnailQuality";
        internal const string kThumbnailSize = "ThumbnailSize";
        internal const string kUserFilesDir = "UserFilesDir";

        public static string GetConfig(string key, string defaultValue = null)
        {
            return WebConfigurationManager.AppSettings[key] ?? defaultValue;
        }

        static string DefaultAllowForgotPassword { get { return GetConfig(kAllowForgotPassword, bool.FalseString); } }

        static string DefaultAllowUserRegistration { get { return GetConfig(kAllowUserRegistration, bool.FalseString); } }

        static string DefaultShowSharedFoldersInMenus { get { return GetConfig(kShowSharedFoldersInMenus, bool.TrueString); } }

        static string DefaultTempDir { get { return GetConfig(kTempDir) ?? "~/App_Data/temp".ResolveLocalPath(); } }

        static string DefaultUserFilesDir { get { return GetConfig(kUserFilesDir) ?? "~/UserFiles".ResolveLocalPath(); } }

        static string DefaultSkinDefinitionFile { get { return GetConfig(kSkinDefinitionFile) ?? "~/App_Data/Skins/Default/skin.json"; } }

        public static string GetSettingsFileName(HttpContextBase Context)
        {
            return CacheHelper.GetCacheFileName<Settings>(Context);
        }

        public static Settings Create(HttpContextBase Context)
        {
            var settings = CacheHelper.GetFromCacheOrDefault<Settings>(Context, (value) =>
            {
                string settingsFilename = GetSettingsFileName(Context);
                bool saveFile = false;
                try
                {
                    ApplicationDbContext db = GetDBContext(Context);
                    var list = db.AppSettings.ToList();
                    foreach (var item in list)
                    {
                        value[item.Name] = item.Value;
                    }
                    saveFile = true;
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
                    else
                    {
                        throw;
                    }
                }
                if (saveFile || !File.Exists(settingsFilename))
                {
                    try
                    {
                        File.WriteAllText(settingsFilename, JsonConvert.SerializeObject(value));
                    }
                    catch { }
                }
            });
            settings.Context = Context;
            return settings;
        }

        private static ApplicationDbContext GetDBContext(HttpContextBase Context)
        {
            try
            {
                return Context.GetOwinContext().Get<ApplicationDbContext>();
            }
            catch { }
            return ApplicationDbContext.Create();
        }

        internal static AppSetting[] GetDefaultSettings()
        {
            return new[] {
                new AppSetting { Name = kAllowForgotPassword, Kind = AppSetting.KindBool, DefaultValue = DefaultAllowForgotPassword, Description = "Allows users to reset forgotten passwords" },
                new AppSetting { Name = kAllowUserRegistration, Kind = AppSetting.KindBool, DefaultValue = DefaultAllowUserRegistration, Description = "Opens user registration to the public" },
                new AppSetting { Name = kCanonicalHostName, Kind = AppSetting.KindString, Description = "The host name that should be used for this site" },
                new AppSetting { Name = kCreatePDFVersionsOfDocuments, Kind = AppSetting.KindBool, DefaultValue="False", Description = "Creates a downloadable PDF copy of documents, spreadsheets, and presentations to PDF on the server. Requires an installation of Microsoft Office on the server." },
                new AppSetting { Name = kDefaultPageDescription, Kind = AppSetting.KindString, Description = "The default page description" },
                new AppSetting { Name = kDefaultPageLayout, Kind = AppSetting.KindFile, Description = "The default page layout file" },
                new AppSetting { Name = kDefaultPageTitle, Kind = AppSetting.KindString, Description = "The default page title" },
                new AppSetting { Name = kDefaultSiteImage, Kind = AppSetting.KindUpload, Description = "The default image used for links shared via social media" },
                new AppSetting { Name = kDefaultUploadPermissions, Kind = AppSetting.KindRole, Description = "The default permissions for files uploaded by any user" },
                new AppSetting { Name = kEmailDefaultFrom, Kind = AppSetting.KindString, Description = "The email address used for emails sent by the system" },
                new AppSetting { Name = kMaxEmailSendBatch, Kind = AppSetting.KindNumber, DefaultValue="100", Description = "The maximum number of emails that can be sent during a single SMTP client session" },
                new AppSetting { Name = kRolesWithUploadPermission, Kind = AppSetting.KindRole, Description = "The user roles that can upload files to shared folders" },
                new AppSetting { Name = kSharedFoldersLabel, Kind = AppSetting.KindString, DefaultValue = "Shared Folders", Description = "The text for the Shared Folders links" },
                new AppSetting { Name = kSharedFoldersRootPermissions, Kind = AppSetting.KindRole, DefaultValue = "", Description = "The roles that can view the root of the shared folders directory. Shared Folder links are always available to the roles assigned." },
                new AppSetting { Name = kShowSharedFoldersInMenus, Kind = AppSetting.KindBool, DefaultValue = DefaultShowSharedFoldersInMenus, Description = "Add links to Shared Folders in the navigation. Shared Folder links are always available to the roles assigned." },
                new AppSetting { Name = kShowSignInLinks, Kind = AppSetting.KindBool, DefaultValue = bool.TrueString, Description = "Add links to the sign in form in the navigation. This does not disable user sign in." },
                new AppSetting { Name = kSiteName, Kind = AppSetting.KindString, Description = "The name of the site" },
                new AppSetting { Name = kSiteOwner, Kind = AppSetting.KindString, Description = "The name of the site owner" },
                new AppSetting { Name = kSkinDefinitionFile, Kind = AppSetting.KindFile, Description = "The site skin definition file" },
                new AppSetting { Name = kTempDir, Kind = AppSetting.KindDirectory, Description = "The directory used for temporary files" },
                new AppSetting { Name = kThumbnailQuality, Kind = AppSetting.KindNumber, DefaultValue = "70", Description = "(10 - 100) The image quality for thumbnails created for uploaded files." },
                new AppSetting { Name = kThumbnailSize, Kind = AppSetting.KindNumber, DefaultValue = "320", Description = "The width of thumbnail images created for uploaded files." },
                new AppSetting { Name = kUserFilesDir, Kind = AppSetting.KindDirectory, Description = "The directory used for files uploaded by users" }
            };
        }
    }
}
