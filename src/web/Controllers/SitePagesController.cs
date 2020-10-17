using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using wwwplatform.Models;
using wwwplatform.Extensions;
using wwwplatform.Shared.Extensions.System;
using wwwplatform.Shared.Extensions;
using HtmlAgilityPack;
using System.Web.WebPages;
using Elmah.Assertions;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace wwwplatform.Controllers
{
    [Extensions.Attributes.Authorize(Roles.Editors, Roles.Administrators)]
    public class SitePagesController : BaseController
    {
        const string AllowedFields = "Id,Name,Order,Slug,Description,PubDate,Layout,HTMLBody,HTMLHeaders,ParentPageId,ShowInNavigation,HomePage,RedirectUrl";

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
            if (db.SitePages.Any(p => p.Slug == sitePage.Slug))
            {
                ModelState.AddModelError("Slug", "Slug is already being used");
            }
            if (!IsValidLayout(sitePage.Layout))
            {
                ModelState.AddModelError("Layout", $"{sitePage.Layout} was not found");
            }
            if (ModelState.IsValid)
            {
                sitePage.PubDate = sitePage.PubDate.FromTimezone(UserTimeZoneOffset);
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
            sitePage.PubDate = sitePage.PubDate.ToTimezone(UserTimeZoneOffset);
            List<string> list;
            if (GetStyleSheetsFromLayout(sitePage.Layout, out list))
            {
                ViewBag.StylesheetsFromLayout = JsonConvert.SerializeObject(list);
            }
            return View(sitePage);
        }

        // POST: SitePages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = AllowedFields)] SitePage sitePage, string[] permissions)
        {
            if (db.SitePages.Any(p => p.Id != sitePage.Id && p.Slug == sitePage.Slug))
            {
                ModelState.AddModelError("Slug", "Slug is already being used");
            }
            if (!IsValidLayout(sitePage.Layout))
            {
                ModelState.AddModelError("Layout", $"{sitePage.Layout} was not found");
            }
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
            List<string> list;
            if (GetStyleSheetsFromLayout(sitePage.Layout, out list))
            {
                ViewBag.StylesheetsFromLayout = JsonConvert.SerializeObject(list);
            }
            return View(sitePage);
        }

        // GET: SitePages/Copy/5
        public async Task<ActionResult> Copy(long? id)
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
            var copy = sitePage.CreateCopy();
            Permission.Apply(db, User, RoleManager, copy, sitePage.Permissions.Select(p => p.AppliesToRole_Id));
            db.SitePages.Add(copy);
            await db.SaveChangesAsync();

            return RedirectToAction("Edit", new { copy.Id });
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
                db.Permissions.RemoveRange(sitePage.Permissions);
                db.SitePages.Remove(sitePage);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Display(string slug)
        {
            var pages = SitePage.GetAvailablePages(db, User, UserManager, RoleManager, true, false, false);
            var sitePage = await pages.Where(p => p.Slug == slug).FirstOrDefaultAsync();

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

        private bool IsValidLayout(string layout)
        {
            var viewResult = ViewEngines.Engines.FindView(ControllerContext, "Index", layout);
            return viewResult.View != null;
        }

        private bool GetStyleSheetsFromLayout(string layout, out List<string> list)
        {
            list = null;
            if (!layout.IsEmpty())
            {
                list = new List<string>();
                try
                {
                    var html = RenderRazorViewToString(layout, new List<SitePage>());
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    var nodes = doc.DocumentNode.SelectNodes("//html/head/link[@rel='stylesheet']");
                    foreach (var node in nodes)
                    {
                        var css = node.GetAttributeValue("href", "");
                        if (!css.IsEmpty())
                        {
                            list.Add(css);
                        }
                    }
                    return true;
                }
                catch { }
            }
            return false;
        }

        private void PreparePage(SitePage sitePage, string[] permissions = null)
        {
            sitePage.Name = sitePage.Name.Trim();
            sitePage.Slug = Extensions.String.Coalesce(sitePage.Slug, sitePage.Name.CleanFileName());

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
