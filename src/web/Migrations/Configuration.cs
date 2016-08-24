
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using wwwplatform.Models;

using Microsoft.AspNet.Identity.Owin;

namespace wwwplatform.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "wwwplatform.Models.ApplicationDbContext";
        }

        //  This method will be called after migrating to the latest version.
        protected override void Seed(ApplicationDbContext context)
        {
            var RoleManager = DependencyResolver.Current.GetService<ApplicationRoleManager>();
            if (RoleManager == null && HttpContext.Current == null) return;
            RoleManager = RoleManager ?? (new HttpContextWrapper(HttpContext.Current)).GetOwinContext().Get<ApplicationRoleManager>();
            var results = Roles.CreateAll(RoleManager);
            foreach (var result in results)
            {
                if (result != null)
                {
                    throw new Exception(String.Join("\r\n", result.Errors.ToArray()));
                }
            }

            try
            {
                if (!context.SitePages.Where(p => p.HomePage).Any())
                {
                    var publicRole = RoleManager.Roles.Where(r => r.Name == Roles.Public).First();
                    var homepage = context.SitePages.Add(new SitePage
                    {
                        Description = "Default Home Page",
                        HomePage = true,
                        HTMLBody = "<h1>Home Page</h1><p>This is the default home page.  You can edit it or set a new home page but you cannot delete it.</p>",
                        Name = "Home Page",
                        Slug = "home-page",
                        PubDate = DateTime.UtcNow
                    });
                    homepage.Permissions = new List<Permission>();
                    homepage.Permissions.Add(new Permission
                    {
                        Grant = true,
                        AppliesToRole_Id = publicRole.Id
                    });
                    context.SaveChanges();
                }
                context.AppSettings.AddOrUpdate(
                    new AppSetting { Name = "AllowForgotPassword", Description = "Allows users to reset forgotten passwords" },
                    new AppSetting { Name = "AllowUserRegistration", Description = "Opens user registration to the public" },
                    new AppSetting { Name = "CanonicalHostName", Description = "The website address that should be used for this site" },
                    new AppSetting { Name = "DefaultPageDescription", Description = "The default page description" },
                    new AppSetting { Name = "DefaultPageTitle", Description = "The default page title" },
                    new AppSetting { Name = "DefaultSiteImage", Description = "The default image used for links shared via social media" },
                    new AppSetting { Name = "EmailDefaultFrom", Description = "The email address used for emails sent by the system" },
                    new AppSetting { Name = "SiteName", Description = "The name of the site" },
                    new AppSetting { Name = "SiteOwner", Description = "The name of the site owner" },
                    new AppSetting { Name = "TempDir", Description = "The directory used for temporary files" },
                    new AppSetting { Name = "UserFilesDir", Description = "The directory used for files uploaded by users" });
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debugger.Break();
                throw exception;
            }
        }

        public void Uninstall(ApplicationDbContext context)
        {
            //TODO: make this work for all database platforms
            string dropFK = @"SELECT distinct
                'IF OBJECT_ID(''['+f.name+']'') IS NOT NULL ALTER TABLE [dbo].['+OBJECT_NAME(f.parent_object_id)+'] DROP CONSTRAINT ['+f.name+']' AS drop_sql
                FROM sys.foreign_keys AS f 
                INNER JOIN sys.foreign_key_columns AS fc 
                ON f.OBJECT_ID = fc.constraint_object_id";

            List<string> results = context.Database.SqlQuery<string>(dropFK).ToList();
            string sql = String.Join(";", results);
            if (sql != "") context.Database.ExecuteSqlCommand(sql);

            string dropTables = @"select 'DROP TABLE [' + sys.schemas.name + '].[' + sys.tables.name + ']' as drop_sql from sys.tables
                join sys.schemas on sys.tables.schema_id = sys.schemas.schema_id WHERE sys.tables.type = 'U'";

            results = context.Database.SqlQuery<string>(dropTables).ToList();
            sql = String.Join(";", results);
            if (sql != "") context.Database.ExecuteSqlCommand(sql);

            string dropSP = @"select 'DROP PROC [' + sys.schemas.name + '].[' + sys.procedures.name + ']' as drop_sql from sys.procedures
                join sys.schemas on sys.procedures.schema_id = sys.schemas.schema_id WHERE sys.procedures.type = 'P'";

            results = context.Database.SqlQuery<string>(dropSP).ToList();
            sql = String.Join(";", results);
            if (sql != "") context.Database.ExecuteSqlCommand(sql);

        }
    }
}
