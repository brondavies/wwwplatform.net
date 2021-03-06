﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web;
using wwwplatform.Shared.Extensions;
using wwwplatform.Shared.Extensions.System;

namespace wwwplatform.Models
{
    public partial class SitePage : Auditable, Permissible
    {
        public SitePage CreateCopy()
        {
            var newname = Name + " (Copy)";
            return new SitePage
            {
                Description = Description,
                HTMLBody = HTMLBody,
                HTMLHeaders = HTMLHeaders,
                Layout = Layout,
                Name = newname,
                Order = Order,
                ParentPageId = ParentPageId,
                PubDate = PubDate,
                RedirectUrl = RedirectUrl,
                ShowInNavigation = ShowInNavigation,
                Slug = newname.CleanFileName(),
            };
        }

        public void Update(SitePage sitePage, int? timeZoneOffset)
        {
            Description = sitePage.Description;
            HomePage = sitePage.HomePage;
            HTMLBody = sitePage.HTMLBody;
            HTMLHeaders = sitePage.HTMLHeaders;
            Layout = sitePage.Layout;
            Name = sitePage.Name;
            Order = sitePage.Order;
            ParentPageId = sitePage.ParentPageId;
            PubDate = (timeZoneOffset.HasValue) ? sitePage.PubDate.FromTimezone(timeZoneOffset.Value) : sitePage.PubDate;
            RedirectUrl = sitePage.RedirectUrl;
            ShowInNavigation = sitePage.ShowInNavigation;
            Slug = sitePage.Slug;
        }

        public string AppRelativeUrl(HttpContextBase context = null)
        {
            return "~/".ResolveUrl(context) + Slug;
        }

        public static IQueryable<SitePage> GetAvailablePages(ApplicationDbContext db, IPrincipal User, ApplicationUserManager UserManager, ApplicationRoleManager RoleManager, bool published = true, bool isParent = true, bool showInNavigation = false, bool includeSubPages = false)
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
            if (includeSubPages)
            {
                pages = pages.Include(p => p.SubPages);
            }

            return pages.OrderBy(p => p.ParentPageId).OrderBy(p => p.Order);
        }
    }
}
