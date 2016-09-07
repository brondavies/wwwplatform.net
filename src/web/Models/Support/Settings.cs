﻿namespace wwwplatform.Models
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

        public string EmailDefaultFrom { get { return GetValue(kEmailDefaultFrom, GetConfig(kEmailDefaultFrom)); } }
        
        public bool ShowSharedFoldersInMenus { get { return bool.Parse(GetValue(kShowSharedFoldersInMenus, DefaultShowSharedFoldersInMenus)); } }

        public string SiteName { get { return GetValue(kSiteName, GetConfig(kSiteName)); } }

        public string SiteOwner { get { return GetValue(kSiteOwner, GetConfig(kSiteOwner)); } }

        public string TempDir { get { return GetValue(kTempDir, DefaultTempDir); } }

        public string UserFilesDir { get { return GetValue(kUserFilesDir, DefaultUserFilesDir); } }
    }
}