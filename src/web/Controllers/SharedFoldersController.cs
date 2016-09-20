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
        private const string AllowedFields = "Id,Name,Description,ParentFolderId";

        // GET: SharedFolders
        [AllowAnonymous]
        public async Task<ActionResult> Index(long? Id)
        {
            if (Settings.SharedFoldersRootPermissions.Contains(PublicRole.Id)
                   || Roles.UserInAnyRole(User, RoleManager, Settings.SharedFoldersRootPermissions.Split(',')))
            {
                var folders = await SharedFolder.GetAvailableFolders(db, User, UserManager, RoleManager, false, true, Id).ToListAsync();
                return Auto(folders);
            }
            else
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl.ToAppPath(HttpContext) });
            }
        }

        // GET: SharedFolders/Create
        [HttpGet]
        public ActionResult Create(long? ParentFolderId)
        {
            return View(new SharedFolder { ParentFolderId = ParentFolderId });
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

                if (sharedFolder.ParentFolderId.HasValue)
                {
                    var ParentFolder = db.SharedFolders.Find(sharedFolder.ParentFolderId);
                    return Redirect("/Shared/" + ParentFolder.Slug);
                }
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

            var sharedFolders = SharedFolder.GetEditableFolders(db, User);
            var sharedFolder = await sharedFolders.FindAsync(id.Value);

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
                var sharedFolders = SharedFolder.GetEditableFolders(db, User);
                SharedFolder actual = await sharedFolders.FindAsync(sharedFolder.Id);
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
            var sharedFolders = SharedFolder.GetEditableFolders(db, User);
            SharedFolder sharedFolder = await sharedFolders.FindAsync(id.Value);
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
            var sharedFolders = SharedFolder.GetEditableFolders(db, User);
            SharedFolder sharedFolder = await sharedFolders.Include(f => f.Permissions).FindAsync(id);
            db.SharedFolders.Remove(sharedFolder);
            await db.SaveChangesAsync();
            SetSuccessMessage(string.Format("Folder {0} was deleted successfully!", sharedFolder.Name));

            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Head)]
        [AllowAnonymous]
        public async Task<ActionResult> Display(string slug)
        {
            string s = slug.ToLowerInvariant();
            var folders = SharedFolder.GetAvailableFolders(db, User, UserManager, RoleManager);
            var folder = await folders.Where(p => p.Slug == s).FirstOrDefaultAsync();

            if (folder == null)
            {
                return HttpNotFound();
            }

            var ids = folder.Files?.Select(f => f.Id) ?? new List<long>();
            folder.Files = WebFile.GetAvailableFiles(db, User, UserManager, RoleManager).Where(f => ids.Contains(f.Id)).ToList();

            ids = folder.SubFolders?.Select(f => f.Id) ?? new List<long>();
            folder.SubFolders = SharedFolder.GetAvailableFolders(db, User, UserManager, RoleManager).Where(f => ids.Contains(f.Id)).ToList();

            return View(folder);
        }

        // POST: SharedFolders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddFiles(long id, long[] files)
        {
            SharedFolder folder = await SharedFolder.GetAvailableFolders(db, User, UserManager, RoleManager).FindAsync(id);
            if (folder == null)
            {
                return HttpNotFound();
            }
            var webfiles = await WebFile.GetAvailableFiles(db, User, UserManager, RoleManager).Where(f => files.Contains(f.Id)).ToListAsync();
            foreach (var wf in webfiles) folder.Files.Add(wf);
            await db.SaveChangesAsync();

            return Json(new { status = "OK" });
        }

        private void PrepareFolder(SharedFolder sharedFolder, string[] permissions)
        {
            sharedFolder.Name = sharedFolder.Name.Trim();
            string cleanName = sharedFolder.Name.CleanFileName();
            string slug = cleanName.ToLowerInvariant();
            if (slug != sharedFolder.Slug)
            {
                var folders = db.ActiveSharedFolders.Where(f => f.Slug.StartsWith(slug) && f.Id != sharedFolder.Id).OrderBy(f => f.Slug).ToList();
                if (folders.Count() > 0)
                {
                    int count = 0;
                    Func<string> suffix = () =>
                    {
                        if (sharedFolder.Id == 0)
                        {
                            count++;
                            return "_" + count;
                        }
                        else
                        {
                            return "-" + sharedFolder.Id;
                        }
                    };
                    foreach (var folder in folders)
                    {
                        if (folder.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
                        {
                            slug = cleanName + suffix();
                            break;
                        }
                    }
                }
            }
            sharedFolder.Slug = slug.ToLowerInvariant();
            Permission.Apply(db, User, RoleManager, sharedFolder, permissions);
        }
    }
}