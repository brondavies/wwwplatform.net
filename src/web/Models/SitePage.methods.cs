using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using wwwplatform.Shared.Extensions;
using wwwplatform.Shared.Extensions.System;

namespace wwwplatform.Models
{
    public partial class SitePage : Auditable, Permissible
    {
        internal void Update(SitePage sitePage, int? timeZoneOffset)
        {
            Name = sitePage.Name;
            Description = sitePage.Description;
            PubDate = sitePage.PubDate;
            if (timeZoneOffset.HasValue)
            {
                PubDate = PubDate.FromTimezone(timeZoneOffset.Value);
            }
            HTMLBody = sitePage.HTMLBody;
            Order = sitePage.Order;
            ParentPageId = sitePage.ParentPageId;
            ShowInNavigation = sitePage.ShowInNavigation;
            HomePage = sitePage.HomePage;
            RedirectUrl = sitePage.RedirectUrl;
        }

        public string AppRelativeUrl(HttpContextBase context = null)
        {
            return "~/".ResolveUrl(context) + Slug;
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
