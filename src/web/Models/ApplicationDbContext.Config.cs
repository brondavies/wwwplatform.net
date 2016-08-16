using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace wwwplatform.Models
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Auditable>();

            MapAuditable<WebFile>(modelBuilder);

            MapAuditable<SharedFolder>(modelBuilder);

            MapAuditable<SitePage>(modelBuilder);

            MapAuditable<EmailAddress>(modelBuilder);

            MapAuditable<EmailMessage>(modelBuilder);

            MapAuditable<MailingList>(modelBuilder);

            MapAuditable<MailingListSubscriber>(modelBuilder);

            modelBuilder.Entity<SharedFolder>()
                .HasMany(m => m.Files)
                .WithOptional()
                .WillCascadeOnDelete(false);

            //Setup Entities with permissions

            modelBuilder.Entity<SharedFolder>()
                .HasMany(m => m.Permissions)
                .WithOptional()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WebFile>()
                .HasMany(m => m.Permissions)
                .WithOptional()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SitePage>()
                .HasMany(m => m.Permissions)
                .WithOptional()
                .WillCascadeOnDelete(false);
                
            base.OnModelCreating(modelBuilder);
        }

        private static EntityTypeConfiguration<T> MapAuditable<T>(DbModelBuilder modelBuilder) where T : Auditable
        {
            var mapping = modelBuilder.Entity<T>();
            mapping.Map(m =>
            {
                m.MapInheritedProperties();
            });
            mapping.Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            return mapping;
        }

        public void Setup()
        {
            Database.CreateIfNotExists();
        }
    }
}
