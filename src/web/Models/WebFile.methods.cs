using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace wwwplatform.Models
{
    public partial class WebFile : Auditable
    {
        internal static IQueryable<WebFile> GetAvailableFiles(ApplicationDbContext db, IPrincipal User, ApplicationUserManager UserManager, ApplicationRoleManager RoleManager)
        {
            var publicRoleId = RoleManager.FindByName(Roles.Public).Id;
            var files = db.ActiveWebFiles;
            if (User.Identity.IsAuthenticated)
            {
                if (!User.IsInRole(Roles.Administrators))
                {
                    var userId = User.Identity.GetUserId();
                    var roleNames = UserManager.GetRoles(User.Identity.GetUserId());
                    var roles = RoleManager.Roles.Where(r => roleNames.Contains(r.Name)).Select(r => r.Id).ToList();
                    roles.Add(publicRoleId);
                    files = files.Where(page => page.Permissions.Any(p => p.Grant &&
                            (p.AppliesTo_Id == userId || roles.Contains(p.AppliesToRole_Id))
                        ));
                }
            }
            else
            {
                files = files.Where(page => page.Permissions.Any(
                    f => f.Grant &&
                    f.AppliesToRole_Id == publicRoleId));
            }

            return files.OrderBy(f => f.Name);
        }
    }
}