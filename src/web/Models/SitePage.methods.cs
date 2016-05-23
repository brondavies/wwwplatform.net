using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using wwwplatform.Shared.Extensions.System;

namespace wwwplatform.Models
{
    public partial class SitePage : Auditable
    {
        internal void Update(SitePage sitePage)
        {
            Name = sitePage.Name;
            Description = sitePage.Description;
            PubDate = sitePage.PubDate;
            HTMLBody = sitePage.HTMLBody;
            Order = sitePage.Order;
            ParentPageId = sitePage.ParentPageId;
            ShowInNavigation = sitePage.ShowInNavigation;
            HomePage = sitePage.HomePage;
        }

        public string AppRelativeUrl()
        {
            return "~/".ResolveUrl() + Slug;
        }

        internal static IQueryable<SitePage> GetAvailablePages(ApplicationDbContext db, IPrincipal User, ApplicationUserManager UserManager, ApplicationRoleManager RoleManager, bool published = true, bool isParent = true, bool showInNavigation = false)
        {
            var publicRoleId = RoleManager.FindByName(Roles.Public).Id;
            var pages = db.ActiveSitePages;
            if (showInNavigation)
            {
                pages = pages.Where(p => p.HomePage == false && p.ShowInNavigation == true);
            }
            if (isParent)
            {
                pages = pages.Where(p => p.ParentPageId == null);
            }
            if (published)
            {
                pages = pages.Where(p => p.PubDate < DateTime.UtcNow);
            }
            if (User.Identity.IsAuthenticated)
            {
                if (!User.IsInRole(Roles.Administrators))
                {
                    var userId = User.Identity.GetUserId();
                    var roleNames = UserManager.GetRoles(User.Identity.GetUserId());
                    var roles = RoleManager.Roles.Where(r => roleNames.Contains(r.Name)).Select(r => r.Id).ToList();
                    roles.Add(publicRoleId);
                    pages = pages.Where(page => page.Permissions.Any(p => p.Grant && 
                        p.ContentType == PermissionContentType.Page &&
                            (p.AppliesTo_Id == userId || roles.Contains(p.AppliesToRole_Id))
                        ));
                }
            }
            else
            {
                pages = pages.Where(page => page.Permissions.Any(
                    p => p.Grant && 
                    p.ContentType == PermissionContentType.Page &&
                    p.AppliesToRole_Id == publicRoleId));
            }

            return pages.OrderBy(p => p.ParentPageId).OrderBy(p => p.Order);
        }
    }
}
