namespace wwwplatform.Models
{
    public partial class Settings : ObjectDictionary
    {
        private T GetValue<T>(string key, T defaultValue)
        {
            T value = default(T);
            try
            {
                if (ContainsKey(key))
                {
                    value = (T)this[key];
                }
            }
            catch { }
            return value != null ? value : defaultValue;
        }

        public bool AllowForgotPassword { get { return bool.Parse(GetValue(kAllowForgotPassword, DefaultAllowForgotPassword)); } }

        public bool AllowUserRegistration { get { return bool.Parse(GetValue(kAllowUserRegistration, DefaultAllowUserRegistration)); } }

        public string CanonicalHostName { get { return GetValue(kCanonicalHostName, GetConfig(kCanonicalHostName)); } }

        public string DefaultPageDescription { get { return GetValue(kDefaultPageDescription, ""); } }

        public string DefaultPageLayout { get { return GetValue(kDefaultPageLayout, GetConfig(kDefaultPageLayout)); } }

        public string DefaultPageTitle { get { return GetValue(kDefaultPageTitle, GetConfig(kDefaultPageTitle)); } }

        public string DefaultSiteImage { get { return GetValue(kDefaultSiteImage, GetConfig(kDefaultSiteImage)); } }

        public string DefaultUploadPermissions { get { return GetValue(kDefaultUploadPermissions, GetConfig(kDefaultUploadPermissions)); } }

        public string EmailAdditionalHeaders { get { return null; } }

        public string EmailDefaultFrom { get { return GetValue(kEmailDefaultFrom, GetConfig(kEmailDefaultFrom)); } }

        public string RolesWithUploadPermission { get { return GetValue(kRolesWithUploadPermission, GetConfig(kRolesWithUploadPermission, "")); } }

        public string SharedFoldersLabel { get { return GetValue(kSharedFoldersLabel, GetConfig(kSharedFoldersLabel, "Shared Folders")); } }

        public string SharedFoldersRootPermissions { get { return GetValue(kSharedFoldersRootPermissions, GetConfig(kSharedFoldersRootPermissions, "")); } }

        public bool ShowSharedFoldersInMenus { get { return bool.Parse(GetValue(kShowSharedFoldersInMenus, DefaultShowSharedFoldersInMenus)); } }

        public bool ShowSignInLinks { get { return bool.Parse(GetValue(kShowSignInLinks, GetConfig(kShowSignInLinks, bool.TrueString))); } }

        public string SiteName { get { return GetValue(kSiteName, GetConfig(kSiteName)); } }

        public string SiteOwner { get { return GetValue(kSiteOwner, GetConfig(kSiteOwner)); } }

        public string SkinDefinitionFile { get { return GetValue(kSkinDefinitionFile, GetConfig(kSkinDefinitionFile)); } }

        public string TempDir { get { return GetValue(kTempDir, DefaultTempDir); } }

        public string UserFilesDir { get { return GetValue(kUserFilesDir, DefaultUserFilesDir); } }
    }
}