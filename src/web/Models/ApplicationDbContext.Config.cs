using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace wwwplatform.Models
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Auditable>();

            modelBuilder.Entity<WebFile>().Map(m =>
            {
                m.MapInheritedProperties();
            });

            modelBuilder.Entity<SitePage>().Map(m =>
            {
                m.MapInheritedProperties();
            });

            modelBuilder.Entity<EmailAddress>().Map(m =>
            {
                m.MapInheritedProperties();
            });

            modelBuilder.Entity<EmailMessage>().Map(m =>
            {
                m.MapInheritedProperties();
            });

            modelBuilder.Entity<MailingList>().Map(m =>
            {
                m.MapInheritedProperties();
            });

            modelBuilder.Entity<MailingListSubscriber>().Map(m =>
            {
                m.MapInheritedProperties();
            });

            base.OnModelCreating(modelBuilder);
        }

        public void Setup()
        {
            Database.CreateIfNotExists();
        }
    }
}
