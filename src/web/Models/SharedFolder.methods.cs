﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

using Microsoft.AspNet.Identity;

namespace wwwplatform.Models
{
    public partial class SharedFolder : Auditable
    {
        internal static IQueryable<SharedFolder> GetAvailableFolders(ApplicationDbContext db, IPrincipal User, ApplicationUserManager UserManager, ApplicationRoleManager RoleManager, bool owned = false)
        {
            var folders = Permission.GetPermissible<SharedFolder>(db, User, UserManager, RoleManager);
            if (owned && !User.IsInRole(Roles.Administrators))
            {
                folders = folders.Where(f => f.UpdatedBy == User.Identity.Name);
            }
            return folders.OrderBy(f => f.Name);
        }

        internal void Update(SharedFolder folder)
        {
            Name = folder.Name;
            Description = folder.Description;
        }

        public bool IsOwner(IPrincipal User)
        {
            return UpdatedBy == User.Identity.Name;
        }

        internal static IQueryable<SharedFolder> GetEditableFolders(ApplicationDbContext db, IPrincipal User)
        {
            var sharedFolders = db.ActiveSharedFolders;
            if (!User.IsInRole(Roles.Administrators))
            {
                sharedFolders = sharedFolders.Where(f => f.UpdatedBy == User.Identity.Name);
            }
            return sharedFolders;
        }
    }
}