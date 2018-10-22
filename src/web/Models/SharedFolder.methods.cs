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
        internal static IQueryable<SharedFolder> GetAvailableFolders(ApplicationDbContext db, IPrincipal User, ApplicationUserManager UserManager, ApplicationRoleManager RoleManager, bool owned = false, bool hasParent = false, long? parent = null)
        {
            var folders = Permission.GetPermissible<SharedFolder>(db, User, UserManager, RoleManager);
            if (hasParent)
            {
                folders = folders.Where(f => f.ParentFolderId == parent);
            }

            if (owned && User.Identity.IsAuthenticated && !User.IsInRole(Roles.Administrators))
            {
                folders = folders.Where(f => f.UpdatedBy == User.Identity.Name);
            }
            return folders.OrderBy(f => f.Name);
        }

        internal void Update(SharedFolder folder)
        {
            Name = folder.Name;
            Description = folder.Description;
            ParentFolderId = folder.ParentFolderId;
            Podcast = folder.Podcast;
            PosterId = folder.PosterId;
            PodcastCategory = folder.PodcastCategory;
            PodcastSubCategory = folder.PodcastSubCategory;
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