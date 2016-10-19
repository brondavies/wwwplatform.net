using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Reflection;
using System.Text;
using wwwplatform.Models;

namespace migrate
{
    class Migrate
    {
        private static string connectionString;
        private static string providerName = "System.Data.SqlClient";

        static void Main(string[] args)
        {
            ParseArguments(args);
            using (AppConfig.Change(GetWebConfig()))
            {
                if (connectionString == null)
                {
                    ApplicationDbContext.Upgrade();
                }
                else
                {
                    var database = new DbConnectionInfo(connectionString, providerName);
                    ApplicationDbContext.Upgrade(database);
                }
            }
        }

        private static void ParseArguments(string[] args)
        {
            foreach (string arg in args)
            {
                if (connectionString == null)
                {
                    connectionString = arg;
                }
                else
                {
                    providerName = arg;
                }
            }
        }

        private static string GetWebConfig()
        {
            string curDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.GetFullPath(Path.Combine(curDir, "..\\App_Data")));
            return Path.GetFullPath(Path.Combine(curDir, "..\\web.config"));
        }
    }
}
