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
using Newtonsoft.Json;
using wwwplatform.Models.ViewModels;
using System.Data.Entity;
using wwwplatform.Extensions.Helpers;
using System.Web.WebPages;
using System.Threading;
using System.Linq.Expressions;

namespace wwwplatform.Extensions
{
    public abstract class BaseWebViewPage<T> : WebViewPage<T>
    {
        public readonly FormMethod POST = FormMethod.Post;

        protected override void ConfigurePage(WebPageBase parentPage)
        {
            base.ConfigurePage(parentPage);
            if (null == ViewBag.Layout)
            {
                ViewBag.Layout = Settings.DefaultPageLayout;
            }
        }

        private string _PageCssClass;
        /// <summary>
        /// A CSS class that can be set at the page level by any view
        /// </summary>
        public string PageCssClass
        {
            get { return _PageCssClass ?? string.Format("{0}_{1}", Request.RequestContext.RouteData.Values["controller"], Request.RequestContext.RouteData.Values["action"]); }
            set { _PageCssClass = value; }
        }

        private Settings _Settings;
        public Settings Settings
        {
            get
            {
                if (_Settings == null)
                {
                    _Settings = Settings.Create(Context);
                }
                return _Settings;
            }
        }

        private int? _UserTimeZoneOffset;
        public int UserTimeZoneOffset
        {
            get
            {
                if (_UserTimeZoneOffset.HasValue)
                {
                    return _UserTimeZoneOffset.Value;
                }
                var cookie = Context.Request.Cookies.Get("_tz");
                if (cookie != null)
                {
                    int result;
                    if (int.TryParse(cookie.Value, out result))
                    {
                        _UserTimeZoneOffset = result;
                        return result;
                    }
                }
                return 0;
            }
        }

        #region Helpers

        public IHtmlString ToJson(object model)
        {
            return Html.Raw(JsonConvert.SerializeObject(model));
        }

        public IHtmlString LI(IHtmlString inner)
        {
            return new HtmlString("<li>" + inner + "</li>");
        }

        #endregion

        #region Role-based links

        public IHtmlString LoginLink(string text = null, string className = null)
        {
            if (Request.IsAuthenticated)
            {
                text = text ?? "Sign Out";
                var form = Html.BeginForm("LogOff", "Account", POST, new { id = "logoutForm" });
                var attr = (className != null) ? "class=\"\"" + className : "";
                Write(SimpleAntiForgeryToken());
                WriteLiteral(string.Format("<a href=\"javascript:document.getElementById('logoutForm').submit()\" {0}>{1}</a>", attr, text));
                form.EndForm();
                return null;
            }
            return Html.ActionLink(linkText: "Sign in", actionName: "Login", controllerName: "Account", routeValues: null, htmlAttributes: new { @class = className });
        }

        public IHtmlString PagesAdminLink()
        {
            if (User.Identity.IsAuthenticated && (User.IsInRole(Roles.Editors) || User.IsInRole(Roles.Administrators)))
            {
                return LI(Html.ActionLink("Pages", "Index", "SitePages"));
            }

            return null;
        }

        public IHtmlString RolesAdminLink()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole(Roles.Administrators))
            {
                return LI(Html.ActionLink("Manage Roles", "Index", "Roles"));
            }

            return null;
        }

        public IHtmlString SettingsAdminLink()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole(Roles.Administrators))
            {
                return LI(Html.ActionLink("Settings", "Index", "AppSettings"));
            }

            return null;
        }

        public IHtmlString UserAdminLink()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole(Roles.Administrators))
            {
                return LI(Html.ActionLink("Manage Users", "Index", "Users"));
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
                return LI(Html.ActionLink("Mailing Lists", "Index", "MailingLists"));
            }

            return null;
        }

        public bool SharedFoldersLinkIsAvailable()
        {
            return (PublicRole != null && Settings.SharedFoldersRootPermissions.Contains(PublicRole.Id))
                || UserInAnyRole(Settings.SharedFoldersRootPermissions.Split(','));
        }

        public IHtmlString SharedFoldersLink()
        {
            return LI(Html.ActionLink(Settings.SharedFoldersLabel, "Index", "SharedFolders"));
        }

        public bool UserInAnyRole(string[] roleIds)
        {
            return Roles.UserInAnyRole(User, RoleManager, roleIds);
        }

        public IHtmlString UploadDialogButton(SelectFileOptions options)
        {
            var uploadRoles = Settings.RolesWithUploadPermission.Split(',');
            if (Roles.UserInAnyRole(User, RoleManager, uploadRoles))
            {
                if (string.IsNullOrEmpty(options.eventName))
                {
                    options.eventName = options.name + ".selected";
                }
                var script = "$(window).trigger('selectfile.show', {eventName: '" + options.eventName + "'});";

                WriteLiteral("<a href=\"#\" class=\"btn btn-default " + options.className + "\" onclick=\"" + script + "\">" + options.text + "</a>");
                return Html.Partial("_SelectFile", options);
            }
            return null;
        }

        public IEnumerable<SelectListItem> CreateSelectList<Y>(IEnumerable<Y> options,
                                                               Expression<Func<Y, bool>> selected,
                                                               Expression<Func<Y, object>> display,
                                                               Expression<Func<Y, object>> value,
                                                               bool required = false) where Y : new()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var selectedEvaluator = selected.Compile();
            var displayEvaluator = display.Compile();
            var valueEvaluator = value.Compile();

            if (!required)
            {
                list.Add(new SelectListItem { Text = "None", Value = "", Selected = selectedEvaluator(new Y()) });
            }
            new List<Y>(options).ForEach((Y option) =>
            {
                var item = new SelectListItem
                {
                    Value = Convert.ToString(valueEvaluator(option)),
                    Text = Convert.ToString(displayEvaluator(option)),
                    Selected = selectedEvaluator(option)
                };
                list.Add(item);
            });
            return list;
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
            var list = CreateSelectList(otherpages, m => m.Id == selected.GetValueOrDefault(), m => m.Name, m => m.Id);
            return list;
        }

        public List<SitePage> Pages
        {
            get
            {
                return SitePage.GetAvailablePages(db, User, UserManager, RoleManager, true, true, true).Include(p => p.SubPages).ToList();
            }
        }

        #endregion

        #region User management

        public ApplicationUser CurrentUser
        {
            get
            {
                return UserManager.FindById(User.Identity.GetUserId());
            }
        }

        public IdentityRole PublicRole
        {
            get
            {
                return RoleManager.FindByName(Roles.Public);
            }
        }

        public IdentityRole UsersRole
        {
            get
            {
                return RoleManager.FindByName(Roles.Users);
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

        public IHtmlString RoleButtonGroup(IEnumerable<string> permissions, bool includePublic = true, string name = "permissions", string className = null)
        {
            var publicRole = PublicRole;
            WriteLiteral(string.Format("<div class=\"btn-group {0}\" data-toggle=\"buttons\">", className));
            foreach (var role in RoleManager.Roles)
            {
                if (role.Id != publicRole.Id || includePublic)
                {
                    RoleCheckbox(permissions, role, name);
                }
            }
            WriteLiteral("</div>");
            return null;
        }

        public IHtmlString RoleCheckbox(IEnumerable<string> permissions, IdentityRole role, string name)
        {
            bool ischecked = permissions.Contains(role.Id);
            string format = "<input {1} name=\"" + name + "\" type=\"checkbox\" autocomplete=\"off\" value=\"{0}\"><span class=\"fa fa-check\"></span>{2}";
            WriteLiteral("<label class=\"btn btn-default" + (ischecked ? " active" : "") + "\">");
            WriteLiteral(string.Format(format, role.Id, ischecked ? "checked=\"checked\"" : "", role.Name));
            WriteLiteral("</label>");
            return null;
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
