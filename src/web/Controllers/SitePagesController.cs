using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using wwwplatform.Models;
using wwwplatform.Extensions;
using System.Text.RegularExpressions;

namespace wwwplatform.Controllers
{
    [Extensions.Authorize(Roles.Editors, Roles.Administrators)]
    public class SitePagesController : BaseController
    {
        const string AllowedFields = "Id,Name,Order,Description,PubDate,HTMLBody,ParentPageId,ShowInNavigation,HomePage";

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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = AllowedFields)] SitePage sitePage)
        {
            if (ModelState.IsValid)
            {
                PreparePage(sitePage);
                db.SitePages.Add(sitePage);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(sitePage);
        }

        private void PreparePage(SitePage sitePage)
        {
            sitePage.Name = sitePage.Name.Trim();
            Regex regex = new Regex("[^A-Z,^a-z,^0-9]");
            string cleanName = string.Join("-", regex.Replace(sitePage.Name, "-").Split("-".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            sitePage.Slug = cleanName;
            if (sitePage.HomePage)
            {
                var allpages = db.ActiveSitePages.Where(p => p.Id != sitePage.Id && p.HomePage == true).ToList();
                foreach (var page in allpages)
                {
                    page.HomePage = false;
                    db.Entry(page).State = EntityState.Modified;
                }
            }
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = AllowedFields)] SitePage sitePage)
        {
            if (ModelState.IsValid)
            {
                SitePage actual = await db.ActiveSitePages.FindAsync(sitePage.Id);
                if (actual == null)
                {
                    return HttpNotFound();
                }
                actual.Update(sitePage);
                PreparePage(actual);
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
            SitePage sitePage = await db.ActiveSitePages.Where(p => p.Slug == s).FirstOrDefaultAsync();
            if (sitePage == null)
            {
                return HttpNotFound();
            }

            return View(sitePage);
        }
    }
}
