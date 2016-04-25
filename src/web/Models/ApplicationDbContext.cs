using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;

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
            // Uncomment the next line to allow automatic migrations or call ApplicationDbContext.Upgrade() when migration should be executed

            //Database.SetInitializer<ApplicationDbContext>(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Migrations.Configuration>());
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

        public DbSet<EmailAddress> EmailAddresses { get; set; }
        public DbSet<EmailMessage> EmailMessages { get; set; }
        public DbSet<MailingList> MailingLists { get; set; }
        public DbSet<MailingListSubscriber> MailingListSubscribers { get; set; }
        public DbSet<SitePage> SitePages { get; set; }
        public DbSet<WebFile> WebFiles { get; set; }

        public DbQuery<T> Active<T>() where T : Auditable
        {
            return (DbQuery<T>)Set<T>().Where(a => a.DeletedAt == null);
        }

        public IQueryable<EmailAddress> ActiveEmailAddresses { get { return Active<EmailAddress>(); } }
        public IQueryable<EmailMessage> ActiveEmailMessages { get { return Active<EmailMessage>(); } }
        public IQueryable<MailingList> ActiveMailingLists { get { return Active<MailingList>(); } }
        public IQueryable<MailingListSubscriber> ActiveMailingListSubscribers { get { return Active<MailingListSubscriber>(); } }
        public IQueryable<SitePage> ActiveSitePages { get { return Active<SitePage>(); } }
        public IQueryable<WebFile> ActiveWebFiles { get { return Active<WebFile>(); } }

        public DbSet<Auditable> Auditables { get; set; }
    }
}
