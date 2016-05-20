using wwwplatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Mvc.Html;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace wwwplatform.Extensions
{
    public abstract class BaseWebViewPage<T> : WebViewPage<T>
    {
        public readonly FormMethod POST = FormMethod.Post;

        #region Role-based links

        public IHtmlString LoginLink(string text = null, string className = null)
        {
            if (Request.IsAuthenticated)
            {
                text = text ?? "Sign Out";
                var form = Html.BeginForm("LogOff", "Account", FormMethod.Post, new { id = "logoutForm" });
                var attr = (className != null) ? "class=\"\"" + className : "";
                Write(SimpleAntiForgeryToken());
                WriteLiteral(string.Format("<a href=\"javascript:document.getElementById('logoutForm').submit()\" {0}>{1}</a>", attr, text));
                form.EndForm();
                return null;
            }
            return Html.ActionLink(linkText: "Sign in", actionName: "Login", controllerName: "Account", routeValues:null, htmlAttributes: new { @class = className });
        }

        public IHtmlString PagesAdminLink()
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole(Roles.Editors) || User.IsInRole(Roles.Administrators)))
            {
                return Html.ActionLink("Pages", "Index", "SitePages");
            }

            return null;
        }

        public IHtmlString UserAdminLink()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole(Roles.Administrators))
            {
                return Html.ActionLink("Manage Users", "Index", "Users");
            }

            return null;
        }

        public IHtmlString MyAccountLink(string text = null, string className = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Html.ActionLink(linkText: text ?? "My Account", actionName: "Index", controllerName: "Manage", routeValues: null, htmlAttributes: new { @class = className });
            }

            return null;
        }

        public IHtmlString MailingListAdminLink()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole(Roles.Administrators))
            {
                return Html.ActionLink("Mailing Lists", "Index", "MailingLists");
            }

            return null;
        }

        #endregion

        #region Simple Anti Forgery Token

        public MvcHtmlString SimpleAntiForgeryToken()
        {
            return (MvcHtmlString)Context.Items[__SimpleAntiForgeryToken] ?? CreateAntiForgeryField();
        }

        private const string __SimpleAntiForgeryToken = "__SimpleAntiForgeryToken";

        private MvcHtmlString CreateAntiForgeryField()
        {
            var token = Html.AntiForgeryToken();
            Context.Items[__SimpleAntiForgeryToken] = token;
            return token;
        }

        #endregion

        #region Site pages
        public IEnumerable<SelectListItem> GetParentPageSelectList(long Id, long? selected)
        {
            var otherpages = db.ActiveSitePages.Where(p => p.HomePage == false && p.Id != Id).ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "None", Value="", Selected = !selected.HasValue });
            otherpages.ForEach((SitePage p) =>
            {
                var item = new SelectListItem();
                item.Value = Convert.ToString(p.Id);
                item.Text = p.Name;
                if (selected.HasValue)
                {
                    item.Selected = p.Id == selected.Value;
                }
                list.Add(item);
            });
            return list;
        }

        public ApplicationUser CurrentUser
        {
            get
            {
                return UserManager.FindById(User.Identity.GetUserId());
            }
        }

        public List<SitePage> Pages
        {
            get
            {
                return SitePage.GetAvailablePages(db, User, UserManager, RoleManager, true, true, true).ToList();
            }
        }

        public IdentityRole PublicRole
        {
            get
            {
                return RoleManager.FindByName(Roles.Public);
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? GetRoleManager();
            }
        }

        private ApplicationRoleManager GetRoleManager()
        {
            _roleManager = DependencyResolver.Current.GetService<ApplicationRoleManager>()
                ?? Context.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            return _roleManager;
        }

        #endregion

        #region User management

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? GetUserManager();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationUserManager GetUserManager()
        {
            _userManager = DependencyResolver.Current.GetService<ApplicationUserManager>()
                ?? Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return _userManager;
        }

        #endregion

        private ApplicationDbContext _db;
        private ApplicationDbContext db
        {
            get
            {
                return _db ?? Context.GetOwinContext().Get<ApplicationDbContext>();
            }
        }

        private ApplicationDbContext GetApplicationDbContext()
        {
            _db = DependencyResolver.Current.GetService<ApplicationDbContext>()
                ?? Context.GetOwinContext().GetUserManager<ApplicationDbContext>();
            return _db;
        }
    }
}
