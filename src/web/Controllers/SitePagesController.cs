using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using wwwplatform.Models;
using wwwplatform.Extensions;
using wwwplatform.Shared.Extensions.System.Collections;
using wwwplatform.Shared.Extensions.System;

namespace wwwplatform.Controllers
{
    [Extensions.Authorize(Roles.Editors, Roles.Administrators)]
    public class SitePagesController : BaseController
    {
        const string AllowedFields = "Id,Name,Order,Description,PubDate,Layout,HTMLBody,ParentPageId,ShowInNavigation,HomePage,RedirectUrl";

        // GET: SitePages
        public async Task<ActionResult> Index()
        {
            return View(await db.ActiveSitePages.Where(p => p.HomePage == false).OrderBy(p => p.Order).ToListAsync());
        }

        // GET: SitePages/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SitePage sitePage = await db.ActiveSitePages.FindAsync(id);
            if (sitePage == null)
            {
                return HttpNotFound();
            }
            return View(sitePage);
        }

        // GET: SitePages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SitePages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = AllowedFields)] SitePage sitePage, string[] permissions)
        {
            if (ModelState.IsValid)
            {
                sitePage.PubDate = sitePage.PubDate.AddMinutes(0 - UserTimeZoneOffset);
                PreparePage(sitePage, permissions);
                db.SitePages.Add(sitePage);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(sitePage);
        }

        // GET: SitePages/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SitePage sitePage;
            if (id.ToLower() == "home")
            {
                sitePage = await db.ActiveSitePages.Where(p => p.HomePage == true).FirstOrDefaultAsync();
                return RedirectToAction("Edit", new { id = sitePage.Id });
            }
            else
            {
                long idValue = Convert.ToInt64(id);
                sitePage = await db.ActiveSitePages.FindAsync(idValue);
            }
            if (sitePage == null)
            {
                return HttpNotFound();
            }
            return View(sitePage);
        }

        // POST: SitePages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = AllowedFields)] SitePage sitePage, string[] permissions)
        {
            if (ModelState.IsValid)
            {
                SitePage actual = await db.ActiveSitePages.FindAsync(sitePage.Id);
                if (actual == null)
                {
                    return HttpNotFound();
                }
                actual.Update(sitePage, UserTimeZoneOffset);
                PreparePage(actual, permissions);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(sitePage);
        }

        // GET: SitePages/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SitePage sitePage = await db.ActiveSitePages.FindAsync(id);
            if (sitePage == null || sitePage.HomePage == true)
            {
                return HttpNotFound();
            }
            return View(sitePage);
        }

        // POST: SitePages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            SitePage sitePage = await db.ActiveSitePages.FindAsync(id);
            if (sitePage.HomePage == false)
            {
                db.SitePages.Remove(sitePage);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Display(string slug)
        {
            string s = slug.ToLower();
            var pages = SitePage.GetAvailablePages(db, User, UserManager, RoleManager, true, false, false);
            var sitePage = await pages.Where(p => p.Slug == s).FirstOrDefaultAsync();

            if (sitePage == null)
            {
                return HttpNotFound();
            }

            if (!string.IsNullOrEmpty(sitePage.RedirectUrl))
            {
                return Redirect(sitePage.RedirectUrl);
            }

            ViewBag.Layout = sitePage.Layout;

            return View(sitePage);
        }

        private void PreparePage(SitePage sitePage, string[] permissions = null)
        {
            sitePage.Name = sitePage.Name.Trim();
            sitePage.Slug = sitePage.Name.CleanFileName();

            if (sitePage.HomePage)
            {
                var allpages = db.ActiveSitePages.Where(p => p.Id != sitePage.Id && p.HomePage == true).ToList();
                foreach (var page in allpages)
                {
                    page.HomePage = false;
                    db.Entry(page).State = EntityState.Modified;
                }
            }

            Permission.Apply(db, User, RoleManager, sitePage, permissions);
        }
    }
}
