using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatformTests.Support
{
    internal class Settings
    {
        private static TestContext testContext;
        
        private static string AppSetting(string name, string defaultValue = null)
        {
            return ContextSetting(name) ?? ConfigurationManager.AppSettings[name] ?? defaultValue;
        }

        public static string RootUrl { get { return AppSetting("RootUrl", "http://localhost:53812/"); } }

        private static string ContextSetting(string name, string defaultValue = null)
        {
            if (testContext != null && testContext.Properties[name] != null)
            {
                return Convert.ToString(testContext.Properties[name]);
            }
            return defaultValue;
        }

        public static void Initialize(TestContext context)
        {
            testContext = context;
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.GetFullPath(AppSetting("TempDir")));
        }
    }
}
