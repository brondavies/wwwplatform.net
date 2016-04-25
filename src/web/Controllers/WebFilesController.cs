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

namespace wwwplatform.Controllers
{
    [Extensions.Authorize(Roles.Administrators)]
    public class WebFilesController : BaseController
    {
        // GET: WebFiles
        public async Task<ActionResult> Index()
        {
            return View(await db.ActiveWebFiles.ToListAsync());
        }

        // GET: WebFiles/Details/5
        public async Task<ActionResult> Details(long? id)
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
            return View(webFile);
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
            UploadResults uploadResults = new UploadResults
            {
                status = UploadResults.Failed,
                message = "No file upload received."
            };
            try
            {
                foreach (string name in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[name];
                    if (file != null && file.ContentLength > 0)
                    {
                        if (ImageHelper.IsImageFile(file))
                        {
                            UploadImage(file, webFile);
                        }
                        else
                        if ((VideoHelper.IsVideoFile(file) || VideoHelper.IsAudioFile(file)))
                        {
                            UploadVideoOrAudio(file, webFile);
                        }
                        else
                        if (DocumentHelper.IsDocumentFile(file))
                        {
                            UploadDocument(file, webFile);
                        }
                        else
                        {
                            uploadResults.status = UploadResults.Failed;
                            uploadResults.message = "The file format was not recognized.";
                        }
                    }
                }
                if (ModelState.IsValid)
                {
                    db.WebFiles.Add(webFile);
                    await db.SaveChangesAsync();
                }
                else
                {
                    uploadResults.status = UploadResults.Failed;
                    uploadResults.message = ErrorsFromModelState(ModelState);
                }
            }
            catch (Exception ex)
            {
                uploadResults.status = UploadResults.Failed;
                uploadResults.message = ex.Message;
            }

            return View(uploadResults);
        }

        private void UploadDocument(HttpPostedFileBase file, WebFile webFile)
        {
            //TODO: Handle document conversion to PDF and thumbnailing
            UploadImage(file, webFile);
        }

        private void UploadVideoOrAudio(HttpPostedFileBase file, WebFile webFile)
        {
            //TODO: Handle video conversion to MP4 and thumbnailing
            UploadImage(file, webFile);
        }

        private void UploadImage(HttpPostedFileBase file, WebFile webFile)
        {
            string extension = Path.GetExtension(file.FileName).ToLower();
            string tempfilename = Extensions.String.Random(16);
            string tempfile = Path.ChangeExtension(Path.Combine(Settings.TempDir, tempfilename), extension);
            file.SaveAs(tempfile);

            if (System.IO.File.Exists(tempfile))
            {
                string FileUrl = FileStorage.Save(new FileInfo(tempfile));
                webFile.Location = FileUrl;
                webFile.Name = webFile.Name ?? Path.GetFileNameWithoutExtension(file.FileName);
            }
        }

        // GET: WebFiles/Edit/5
        public async Task<ActionResult> Edit(long? id)
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
            return View(webFile);
        }

        // POST: WebFiles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,Location")] WebFile webFile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(webFile).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(webFile);
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
            return View(webFile);
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
