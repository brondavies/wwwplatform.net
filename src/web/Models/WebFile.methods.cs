using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

using Microsoft.AspNet.Identity;

namespace wwwplatform.Models
{
    public partial class WebFile : Auditable, Permissible
    {
        internal static IQueryable<WebFile> GetAvailableFiles(ApplicationDbContext db, IPrincipal User, ApplicationUserManager UserManager, ApplicationRoleManager RoleManager)
        {
            var files = Permission.GetPermissible<WebFile>(db, User, UserManager, RoleManager);
            return files.OrderBy(f => f.Name);
        }

        public bool IsOwner(IPrincipal User)
        {
            return UpdatedBy == User.Identity.Name;
        }
    }
}