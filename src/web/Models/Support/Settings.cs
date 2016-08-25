﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace wwwplatform.Models
{
    public partial class Settings : ObjectDictionary
    {
        private T GetValue<T>(string key, T defaultValue)
        {
            try
            {
                return ContainsKey(key) ? (T)this[key] : defaultValue;
            }
            catch { }
            return defaultValue;
        }

        public bool AllowForgotPassword { get { return bool.Parse(GetValue("AllowForgotPassword", DefaultAllowForgotPassword)); } }

        public bool AllowUserRegistration { get { return bool.Parse(GetValue("AllowUserRegistration", DefaultAllowUserRegistration)); } }

        public string CanonicalHostName { get { return GetValue("CanonicalHostName", DefaultCanonicalHostName); } }

        public string DefaultPageDescription { get { return GetValue("DefaultPageDescription", ""); } }

        public string DefaultPageTitle { get { return GetValue("DefaultPageTitle", AppSetting("DefaultPageTitle")); } }

        public string DefaultSiteImage { get { return GetValue("DefaultSiteImage", AppSetting("DefaultSiteImage")); } }

        public string EmailDefaultFrom { get { return GetValue("EmailDefaultFrom", DefaultEmailFrom); } }

        public bool ShowSharedFoldersInMenus { get { return bool.Parse(GetValue("ShowSharedFoldersInMenus", DefaultShowSharedFoldersInMenus)); } }

        public string SiteName { get { return GetValue("SiteName", DefaultSiteName); } }

        public string SiteOwner { get { return GetValue("SiteOwner", DefaultSiteOwner); } }

        public string TempDir { get { return GetValue("TempDir", DefaultTempDir); } }

        public string UserFilesDir { get { return GetValue("UserFilesDir", DefaultUserFilesDir); } }
    }
}