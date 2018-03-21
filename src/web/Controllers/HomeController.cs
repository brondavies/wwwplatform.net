using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using wwwplatform.Extensions;
using wwwplatform.Models;

namespace wwwplatform.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var page = SitePage.GetAvailablePages(db, User, UserManager, RoleManager, true, false, false).Where(p => p.HomePage == true).FirstOrDefault();
            if (page == null)
            {
                if (db.ActiveSitePages.Where(p => p.HomePage == true).Any())
                {
                    return new HttpUnauthorizedResult("Access Denied");
                }
                return RedirectToAction("Setup");
            }

            if (!string.IsNullOrEmpty(page.RedirectUrl))
            {
                return Redirect(page.RedirectUrl);
            }

            ViewBag.Layout = page.Layout;
            return View(page);
        }

        public ActionResult Setup()
        {
            return View();
        }

        public ActionResult Upgrade()
        {
            ApplicationDbContext.Upgrade();
            return RedirectToAction("Index");
        }

        public ActionResult Uninstall()
        {
            if (Request.IsLocal)
            {
                var config = new Migrations.Configuration();
                config.Uninstall(db);
            }
            return RedirectToAction("Index");
        }
    }
}
