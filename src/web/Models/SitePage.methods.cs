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
    public partial class SitePage : Auditable, Permissible
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
            var pages = Permission.GetPermissible<SitePage>(db, User, UserManager, RoleManager);
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

            return pages.OrderBy(p => p.ParentPageId).OrderBy(p => p.Order);
        }
    }
}
