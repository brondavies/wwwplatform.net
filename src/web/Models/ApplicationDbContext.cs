using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;

using Microsoft.AspNet.Identity.EntityFramework;

namespace wwwplatform.Models
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static void RegisterConfig()
        {
            // Set AutoMigrateDatabaseToLatestVersion = "False" in web.config to disable automatic migrations
            // and call ApplicationDbContext.Upgrade() when migration should be executed
            // or use Update-Database in the Package Manager Console
            if (Convert.ToBoolean(Settings.AppSetting("AutoMigrateDatabaseToLatestVersion", bool.FalseString)))
            {
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Migrations.Configuration>());
            }
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public static void Upgrade()
        {
            var configuration = new Migrations.Configuration();
            var migrator = new DbMigrator(configuration);
            migrator.Update();
        }
    }
}
