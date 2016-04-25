using EFHooks;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace wwwplatform.Models
{

    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private DbContextHooks _triggers;
        public DbContextHooks Triggers
        {
            get
            {
                if (_triggers == null)
                {
                    _triggers = new DbContextHooks(this);
                    _triggers.Add(new AuditableInsertHook());
                    _triggers.Add(new AuditableUpdateHook());
                    _triggers.Add(new AuditableDeleteHook());
                }
                return _triggers;
            }
        }

        public override int SaveChanges()
        {
            return Triggers.SaveChanges(base.SaveChanges);
        }

        public override Task<int> SaveChangesAsync()
        {
            return SaveChangesAsync(CancellationToken.None);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Triggers.SaveChangesAsync(base.SaveChangesAsync, cancellationToken);
        }
    }

    internal class AuditFields
    {
        internal static bool SetUpdatedByField(Auditable entity, HookEntityMetadata metadata)
        {
            var user = GetHttpContext().GetOwinContext().Request.User;
            string updater = (user != null && user.Identity.IsAuthenticated) ? user.Identity.Name : null;

            if (String.IsNullOrEmpty(entity.UpdatedBy) || (updater != null && (
                    metadata.State == EntityState.Added ||
                    metadata.State == EntityState.Modified ||
                    metadata.State == EntityState.Deleted)
                )
            )
            {
                entity.UpdatedBy = updater;
                return true;
            }
            return false;
        }

        private static HttpContextBase GetHttpContext()
        {
            return new HttpContextWrapper(HttpContext.Current);
        }

        internal static void SetCreatedAtField(Auditable entity)
        {
            if (entity.CreatedAt == null || entity.CreatedAt == DateTime.MinValue)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            entity.UpdatedAt = entity.CreatedAt;
        }

        internal static void SetUpdatedAtField(Auditable entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
        }
    }

    public class AuditableInsertHook : PreInsertHook<Auditable>
    {
        public override void Hook(Auditable entity, HookEntityMetadata metadata)
        {
            EntityState state = metadata.CurrentContext.Entry(entity).State;
            if (state != EntityState.Detached && state != EntityState.Unchanged)
            {
                AuditFields.SetUpdatedByField(entity, metadata);
                AuditFields.SetCreatedAtField(entity);
            }
        }
    }

    public class AuditableUpdateHook : PreUpdateHook<Auditable>
    {
        public override void Hook(Auditable entity, HookEntityMetadata metadata)
        {
            EntityState state = metadata.CurrentContext.Entry(entity).State;
            if (state != EntityState.Detached && state != EntityState.Unchanged)
            {
                AuditFields.SetUpdatedByField(entity, metadata);
                AuditFields.SetUpdatedAtField(entity);
            }
        }
    }

    public class AuditableDeleteHook : PreDeleteHook<Auditable>
    {
        public override void Hook(Auditable entity, HookEntityMetadata metadata)
        {
            AuditFields.SetUpdatedByField(entity, metadata);
            entity.DeletedAt = DateTime.UtcNow;
        }
    }

    public static class IQueryable
    {
        public static T Find<T>(this IQueryable<T> query, long? id) where T : Auditable
        {
            return query.FirstOrDefault<T>(a => a.Id == id);
        }

        public static Task<T> FindAsync<T>(this IQueryable<T> query, long? id) where T : Auditable
        {
            return query.FirstOrDefaultAsync<T>(a => a.Id == id);
        }
    }
}
