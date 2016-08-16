using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using wwwplatform.Shared.Extensions.System.Collections;

namespace wwwplatform.Models
{
    public partial class Permission
    {
        internal static IQueryable<T> GetPermissible<T>(ApplicationDbContext db, IPrincipal User, ApplicationUserManager UserManager, ApplicationRoleManager RoleManager)
            where T : Auditable, Permissible
        {
            var publicRoleId = RoleManager.FindByName(Roles.Public).Id;
            var items = (IQueryable<T>)db.Active<T>();
            if (User.Identity.IsAuthenticated)
            {
                if (!User.IsInRole(Roles.Administrators))
                {
                    var userId = User.Identity.GetUserId();
                    var roleNames = UserManager.GetRoles(User.Identity.GetUserId());
                    var roles = RoleManager.Roles.Where(r => roleNames.Contains(r.Name)).Select(r => r.Id).ToList();
                    roles.Add(publicRoleId);
                    items = items.Where(m => m.Permissions.Any(p => p.Grant &&
                            (p.AppliesTo_Id == userId || roles.Contains(p.AppliesToRole_Id))
                        ));
                }
            }
            else
            {
                items = items.Where(m => m.Permissions.Any(
                    item => item.Grant &&
                    item.AppliesToRole_Id == publicRoleId));
            }
            return items;
        }

        internal static void Apply(ApplicationDbContext db, IPrincipal User, ApplicationRoleManager RoleManager, Permissible permissible, string[] permissions = null)
        {
            if (permissions == null)
            {
                permissions = new string[] { };
            }

            var roles = RoleManager.Roles.ToList();
            if (permissible.Permissions == null)
            {
                permissible.Permissions = new List<Permission>();
            }
            var removed = permissible.Permissions.RemoveAll(p => !permissions.Contains(p.AppliesToRole_Id));
            if (removed.Count() > 0)
            {
                db.Permissions.RemoveRange(removed);
            }
            foreach (var role in roles)
            {
                if (permissions.Contains(role.Id) && !permissible.Permissions.Any(p => p.AppliesToRole_Id == role.Id))
                {
                    permissible.Permissions.Add(db.Permissions.Add(new Permission
                    {
                        AppliesToRole_Id = role.Id,
                        UpdatedBy = User.Identity.Name
                    }));
                }
            }
        }
    }
}