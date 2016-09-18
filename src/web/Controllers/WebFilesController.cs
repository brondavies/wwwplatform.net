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
using wwwplatform.Shared.Helpers;
using System.IO;
using wwwplatform.Extensions.Helpers;
using wwwplatform.Models.ViewModels;
using wwwplatform.Extensions.Attributes;
using wwwplatform.Models.Serializers;
using Microsoft.AspNet.Identity;

namespace wwwplatform.Controllers
{
    [Extensions.Authorize]
    [Serializer(typeof(WebFileSerializer))]
    public class WebFilesController : BaseController
    {
        // GET: WebFiles
        public async Task<ActionResult> Index()
        {
            return Auto(await WebFile.GetAvailableFiles(db, User, UserManager, RoleManager).ToListAsync());
        }

        // GET: WebFiles/Details/5
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Head)]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.Client, Duration = 3600)]
        public async Task<ActionResult> Details(long? id, int v = 0)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebFile webFile = await WebFile.GetAvailableFiles(db, User, UserManager, RoleManager).FindAsync(id);
            if (webFile == null)
            {
                return HttpNotFound();
            }
            string inline = (v == 1) ? "inline" : "attachment";
            string filename = webFile.Name.Replace(" ", "-") + Path.GetExtension(webFile.Location);
            string filepath = Server.MapPath(webFile.Location);
            string contentType = ContentTypeHelper.GetContentType(webFile.Location);
            Response.Headers["Content-Disposition"] = inline + ";filename=" + filename;
            return File(System.IO.File.OpenRead(filepath), contentType);
            //return File(filepath, contentType, filename);
        }

        // GET: WebFiles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebFiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,Location")] WebFile webFile)
        {
            UploadResults results = new UploadResults
            {
                file = webFile,
                message = "No file upload received.",
                status = UploadResults.Failed
            };
            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase uploadedFile = Request.Files[0];
                    if (uploadedFile != null && uploadedFile.ContentLength > 0)
                    {
                        ModelState["Name"]?.Errors?.Clear();
                        if (ImageHelper.IsImageFile(uploadedFile))
                        {
                            UploadImage(uploadedFile, results);
                        }
                        else
                        if ((VideoHelper.IsVideoFile(uploadedFile) || VideoHelper.IsAudioFile(uploadedFile)))
                        {
                            UploadVideoOrAudio(uploadedFile, results);
                        }
                        else
                        if (DocumentHelper.IsDocumentFile(uploadedFile))
                        {
                            UploadDocument(uploadedFile, results);
                        }
                        else
                        {
                            results.status = UploadResults.Failed;
                            results.message = "The file format was not recognized or is not an allowed file type.";
                        }
                    }
                }
                if (ModelState.IsValid)
                {
                    results.file = db.WebFiles.Add(results.file);
                    string defaultRoleSetting = Settings.DefaultUploadPermissions;
                    string[] defaultRoles;
                    if (!string.IsNullOrEmpty(defaultRoleSetting))
                    {
                        defaultRoles = defaultRoleSetting.Split(',');
                    }
                    else
                    {
                        defaultRoles = new string[] { RoleManager.FindByName(Roles.Users)?.Id };
                    }
                    Permission.Apply(db, User, RoleManager, results.file, defaultRoles); //default permissions
                    await db.SaveChangesAsync();
                    results.status = UploadResults.OK;
                    results.message = null;  //"File uploaded successfully.";
                }
                else
                {
                    results.status = UploadResults.Failed;
                    results.message = ErrorsFromModelState(ModelState);
                }
            }
            catch (Exception ex)
            {
                results.status = UploadResults.Failed;
                results.message = ex.Message;
            }

            return Auto(results, "Created");
        }

        private void UploadDocument(HttpPostedFileBase file, UploadResults uploadResults)
        {
            //TODO: Handle document conversion to PDF and thumbnailing
            UploadImage(file, uploadResults);
        }

        private void UploadVideoOrAudio(HttpPostedFileBase file, UploadResults uploadResults)
        {
            //TODO: Handle video conversion to MP4 and thumbnailing
            UploadImage(file, uploadResults);
        }

        private void UploadImage(HttpPostedFileBase file, UploadResults uploadResults)
        {
            string extension = Path.GetExtension(file.FileName).ToLower();
            string tempfilename = Extensions.String.Random(16);
            string tempfile = Path.ChangeExtension(Path.Combine(Path.GetFullPath(Settings.TempDir), tempfilename), extension);
            file.SaveAs(tempfile);

            if (System.IO.File.Exists(tempfile))
            {
                string FileUrl = FileStorage.Save(new FileInfo(tempfile), HttpContext);
                uploadResults.file.Location = FileUrl;
                uploadResults.file.Name = Extensions.String.Coalesce(uploadResults.file.Name, Path.GetFileNameWithoutExtension(file.FileName));
                uploadResults.file.Size = (new FileInfo(tempfile)).Length;
                uploadResults.status = UploadResults.OK;
            }
            else
            {
                uploadResults.status = UploadResults.Failed;
                uploadResults.message = "The file could not be saved.";
            }
        }

        // GET: WebFiles/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebFile webFile = await WebFile.GetAvailableFiles(db, User, UserManager, RoleManager).FindAsync(id);
            if (webFile == null)
            {
                return HttpNotFound();
            }
            return Auto(webFile);
        }

        // POST: WebFiles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,Location")] WebFile webFile, string[] permissions = null)
        {
            if (ModelState.IsValid)
            {
                db.Entry(webFile).State = EntityState.Modified;
                Permission.Apply(db, User, RoleManager, webFile, permissions);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return Auto(webFile);
        }

        // GET: WebFiles/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebFile webFile = await db.ActiveWebFiles.FindAsync(id);
            if (webFile == null)
            {
                return HttpNotFound();
            }
            return Auto(webFile);
        }

        // POST: WebFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            WebFile webFile = await db.ActiveWebFiles.FindAsync(id);
            db.WebFiles.Remove(webFile);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
