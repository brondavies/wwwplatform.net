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

namespace wwwplatform.Extensions
{
    public abstract class BaseWebViewPage<T> : WebViewPage<T>
    {
        public readonly FormMethod POST = FormMethod.Post;

        #region Role-based links

        public IHtmlString LoginLink()
        {
            if (Request.IsAuthenticated)
            {
                var form = Html.BeginForm("LogOff", "Account", FormMethod.Post, new { id = "logoutForm" });
                Write(Html.AntiForgeryToken());
                WriteLiteral(@"<a href=""javascript:document.getElementById('logoutForm').submit()"">Sign out</a>");
                form.EndForm();
                return null;
            }
            return Html.ActionLink("Sign in", "Login", "Account");
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
                return Html.ActionLink("Manage Users", "Index", "Manage");
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
            return (MvcHtmlString)HttpContext.Current.Items[__SimpleAntiForgeryToken] ?? CreateAntiForgeryField();
        }

        private const string __SimpleAntiForgeryToken = "__SimpleAntiForgeryToken";
        private MvcHtmlString CreateAntiForgeryField()
        {
            var token = Html.AntiForgeryToken();
            HttpContext.Current.Items[__SimpleAntiForgeryToken] = token;
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

        public List<SitePage> Pages
        {
            get
            {
                return db.ActiveSitePages
                    .Where(p => p.HomePage == false && p.ShowInNavigation == true)
                    .Where(p => p.PubDate < DateTime.UtcNow && p.ParentPageId == null)
                    .OrderBy(p => p.Order)
                    .ToList();
            }
        }

        #endregion

        private ApplicationDbContext db
        {
            get
            {
                return Context.GetOwinContext().Get<ApplicationDbContext>();
            }
        }
    }
}
