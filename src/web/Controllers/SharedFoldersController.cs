using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using wwwplatform.Extensions;
using wwwplatform.Models;
using wwwplatform.Shared.Extensions.System;

namespace wwwplatform.Controllers
{
    [Extensions.Authorize]
    public class SharedFoldersController : BaseController
    {
        private const string AllowedFields = "Id,Name,Description";

        // GET: SharedFolders
        public async Task<ActionResult> Index()
        {
            var folders = await SharedFolder.GetAvailableFolders(db, User, UserManager, RoleManager).ToListAsync();
            return View(folders);
        }

        // GET: SharedFolders/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: SharedFolders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = AllowedFields)] SharedFolder sharedFolder, string[] permissions)
        {
            if (ModelState.IsValid)
            {
                PrepareFolder(sharedFolder, permissions);
                db.SharedFolders.Add(sharedFolder);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(sharedFolder);
        }

        // GET: SharedFolders/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SharedFolder sharedFolder;
            sharedFolder = await db.ActiveSharedFolders.FindAsync(id.Value);
            
            if (sharedFolder == null)
            {
                return HttpNotFound();
            }
            return View(sharedFolder);
        }

        // POST: SharedFolders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = AllowedFields)] SharedFolder sharedFolder, string[] permissions)
        {
            if (ModelState.IsValid)
            {
                SharedFolder actual = await db.ActiveSharedFolders.FindAsync(sharedFolder.Id);
                if (actual == null)
                {
                    return HttpNotFound();
                }
                actual.Update(sharedFolder);
                PrepareFolder(actual, permissions);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(sharedFolder);
        }
        
        // GET: SharedFolders/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SharedFolder sharedFolder = await db.ActiveSharedFolders.FindAsync(id);
            if (sharedFolder == null)
            {
                return HttpNotFound();
            }
            return View(sharedFolder);
        }

        // POST: SharedFolders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            SharedFolder sharedFolder = await db.ActiveSharedFolders.FindAsync(id);
            db.SharedFolders.Remove(sharedFolder);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Head)]
        public async Task<ActionResult> Display(string slug)
        {
            string s = slug.ToLower();
            var folders = SharedFolder.GetAvailableFolders(db, User, UserManager, RoleManager);
            var folder = await folders.Where(p => p.Slug == s).FirstOrDefaultAsync();

            if (folder == null)
            {
                return HttpNotFound();
            }

            return View(folder);
        }

        private void PrepareFolder(SharedFolder sharedFolder, string[] permissions)
        {
            sharedFolder.Name = sharedFolder.Name.Trim();
            sharedFolder.Slug = sharedFolder.Name.CleanFileName();
            
            Permission.Apply(db, User, RoleManager, sharedFolder, permissions);
        }
    }
}