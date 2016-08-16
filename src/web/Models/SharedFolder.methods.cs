using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

using Microsoft.AspNet.Identity;

namespace wwwplatform.Models
{
    public partial class SharedFolder : Auditable
    {
        internal static IQueryable<SharedFolder> GetAvailableFolders(ApplicationDbContext db, IPrincipal User, ApplicationUserManager UserManager, ApplicationRoleManager RoleManager)
        {
            var folders = Permission.GetPermissible<SharedFolder>(db, User, UserManager, RoleManager);
            return folders.OrderBy(f => f.Name);
        }
    }
}