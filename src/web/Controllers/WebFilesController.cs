﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;

using wwwplatform.Models;
using wwwplatform.Extensions;
using wwwplatform.Shared.Helpers;
using wwwplatform.Extensions.Helpers;
using wwwplatform.Models.ViewModels;
using wwwplatform.Extensions.Attributes;
using wwwplatform.Models.Serializers;

using Microsoft.AspNet.Identity;
using FTTLib;
using wwwplatform.Shared.Extensions;
using ImageProcessor.Imaging.Formats;
using System.Drawing;
using ImageProcessor;
using wwwplatform.Shared.Extensions.System;

namespace wwwplatform.Controllers
{
    [Extensions.Authorize]
    [Serializer(typeof(WebFileSerializer))]
    public class WebFilesController : BaseController
    {
        private const string AllowedFields = "Id,Name,Description,DisplayDate,Location";

        // GET: WebFiles
        public async Task<ActionResult> Index()
        {
            return Auto(await WebFile.GetAvailableFiles(db, User, UserManager, RoleManager).ToListAsync());
        }

        // GET: WebFiles/Details/5
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Head)]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.Client, Duration = 3600)]
        [AllowAnonymous]
        public async Task<ActionResult> Details(long id, string extra = null, int v = 0)
        {
            WebFile webFile = await WebFile.GetAvailableFiles(db, User, UserManager, RoleManager).FindAsync(id);
            if (webFile == null)
            {
                return HttpNotFound();
            }

            string location = webFile.Location;
            if (extra == "preview")
            {
                if (!string.IsNullOrEmpty(webFile.PreviewLocation))
                {
                    location = webFile.PreviewLocation;
                }
                extra = null;
            }
            string filepath = Server.MapPath(location);
            string contentType = FTT.GetMimeType(location ?? "");
            if (contentType == "") { contentType = "application/octet-stream"; }
            if (extra == null)
            {
                string inline = (v == 1) ? "inline" : "attachment";
                string filename = webFile.Name.Replace(" ", "-") + Path.GetExtension(location);
                Response.Headers["Content-Disposition"] = inline + ";filename=" + filename;
            }
            return File(System.IO.File.OpenRead(ReconcileFileToDownload(extra, filepath)), contentType);
            //return File(filepath, contentType, filename);
        }

        private string ReconcileFileToDownload(string prefferedName, string filename)
        {
            if (!string.IsNullOrEmpty(prefferedName))
            {
                string extension = Path.GetExtension(prefferedName);
                string newfilename = Path.ChangeExtension(filename, extension);
                if (System.IO.File.Exists(newfilename))
                {
                    return newfilename;
                }
            }
            return filename;
        }

        // GET: WebFiles/Create
        public ActionResult Create()
        {
            return View(new WebFile());
        }

        // POST: WebFiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = AllowedFields)] WebFile webFile, bool redir = false)
        {
            UploadResults results = new UploadResults
            {
                message = "No file upload received.",
                status = UploadResults.Failed
            };
            try
            {
                if (Request.Files.Count > 0)
                {
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
                    
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase uploadedFile = Request.Files[i];

                        if (uploadedFile != null && uploadedFile.ContentLength > 0)
                        {
                            WebFile file = null;
                            ModelState["Name"]?.Errors?.Clear();
                            if (ImageHelper.IsImageFile(uploadedFile))
                            {
                                file = UploadFile(uploadedFile, results, db, HttpContext, Settings);
                            }
                            else
                            if ((VideoHelper.IsVideoFile(uploadedFile) || VideoHelper.IsAudioFile(uploadedFile)))
                            {
                                file = UploadVideoOrAudio(uploadedFile, results);
                            }
                            else
                            if (DocumentHelper.IsDocumentFile(uploadedFile))
                            {
                                file = UploadDocument(uploadedFile, results);
                            }
                            else
                            {
                                results.status = UploadResults.Failed;
                                results.message = "The file format was not recognized or is not an allowed file type.";
                            }

                            if (file != null)
                            {
                                if (webFile != null)
                                {
                                    file.Update(webFile, UserTimeZoneOffset);
                                    webFile = null;
                                }
                                Permission.Apply(db, User, RoleManager, file, defaultRoles); //default permissions

                                db.WebFiles.Add(file);
                                await db.SaveChangesAsync();

                                results.files.Add(file);
                            }
                        }
                    }


                    results.status = UploadResults.OK;
                    results.message = null;  //"File uploaded successfully.";
                    foreach (var wf in results.files)
                    {
                        if (!string.IsNullOrEmpty(webFile?.Name))
                        {
                            wf.Name = webFile.Name;
                        }
                        else
                        {
                            wf.Name = wf.GetFileName();
                        }
                    }
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

            if (redir)
            {
                if (results.status == UploadResults.Failed)
                {
                    SetFailureMessage(results.message);
                    return View(webFile);
                }
                else
                {
                    SetSuccessMessage(webFile.Name + " uploaded successfully!");
                    return RedirectToAction("Index");
                }
            }
            return Auto(results, "Created");
        }
        
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Error(string id)
        {
            return Auto(new UploadResults {
                message = "File upload failed. Make sure your application is configured correctly for file uploads",
                status = UploadResults.Failed
            });
        }

        private WebFile UploadDocument(HttpPostedFileBase file, UploadResults uploadResults)
        {
            WebFile webfile = UploadFile(file, uploadResults, db, HttpContext, Settings);
            string docFile = Server.MapPath(webfile.Location);
            string pdfFile = Path.ChangeExtension(docFile, ".pdf");
            bool createPdf = !docFile.Equals(pdfFile, StringComparison.InvariantCultureIgnoreCase);
            if (createPdf && uploadResults.status == UploadResults.OK && Settings.CreatePDFVersionsOfDocuments)
            {
                DocumentHelper.CreatePDF(Settings.ConvertPdfExe, docFile, pdfFile, true);
            }
            if (System.IO.File.Exists(pdfFile))
            {
                webfile.PreviewLocation = "/" + CreatePdfThumbnail(pdfFile).ToAppPath(HttpContext);
            }
            return webfile;
        }

        private WebFile UploadVideoOrAudio(HttpPostedFileBase file, UploadResults uploadResults)
        {
            //TODO: Handle video conversion to MP4 and thumbnailing
            return UploadFile(file, uploadResults, db, HttpContext, Settings);
        }

        private WebFile UploadImage(HttpPostedFileBase file, UploadResults uploadResults)
        {
            var webfile = UploadFile(file, uploadResults, db, HttpContext, Settings);
            if (uploadResults.status == UploadResults.OK)
            {
                webfile.PreviewLocation = "/" + CreateThumbnail(Server.MapPath(webfile.Location)).ToAppPath(HttpContext);
            }
            return webfile;
        }

        internal static WebFile UploadFile(HttpPostedFileBase file, UploadResults uploadResults, ApplicationDbContext db, HttpContextBase context, Settings settings)
        {
            string extension = Path.GetExtension(file.FileName).ToLower();
            string tempfilename = Extensions.String.Random(16);
            string tempfile = Path.ChangeExtension(Path.Combine(Path.GetFullPath(settings.TempDir), tempfilename), extension);
            file.SaveAs(tempfile);
            WebFile webfile = db.WebFiles.Create();
            if (System.IO.File.Exists(tempfile))
            {
                string FileUrl = FileStorage.Save(new FileInfo(tempfile), context);
                webfile.Location = FileUrl;
                webfile.Name = Extensions.String.Coalesce(webfile.Name, Path.GetFileNameWithoutExtension(file.FileName));
                webfile.Size = (new FileInfo(tempfile)).Length;
                uploadResults.status = UploadResults.OK;
            }
            else
            {
                uploadResults.status = UploadResults.Failed;
                uploadResults.message = "The file could not be saved.";
            }

            return webfile;
        }

        private string CreateThumbnail(string filename)
        {
            byte[] photoBytes = System.IO.File.ReadAllBytes(filename);
            ISupportedImageFormat format = new JpegFormat { Quality = Settings.ThumbnailQuality };
            Size size = new Size(Settings.ThumbnailSize, 0);
            string thumbFile = GetThumbFileName(filename, size.Width);
            using (var stream = System.IO.File.OpenRead(filename))
            {
                using (var output = System.IO.File.OpenWrite(thumbFile))
                {
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        imageFactory.Load(stream)
                                    .Resize(size)
                                    .Format(format)
                                    .Save(output);
                    }
                }
            }
            return thumbFile;
        }

        private static string GetThumbFileName(string filename, int width)
        {
            return Path.ChangeExtension(filename, ".w" + width + ".jpg");
        }

        private string CreatePdfThumbnail(string filename)
        {
            string thumbFile = GetThumbFileName(filename, Settings.ThumbnailSize);
            DocumentHelper.CreateThumbnail(Settings.ConvertPdfExe, filename, thumbFile, Settings.ThumbnailSize, true);
            return thumbFile;
        }

        // GET: WebFiles/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebFile webFile = await WebFile.GetAvailableFiles(db, User, UserManager, RoleManager, !User.IsInRole(Roles.Administrators)).FindAsync(id);
            if (webFile == null)
            {
                return HttpNotFound();
            }
            if (webFile.DisplayDate.HasValue)
            {
                webFile.DisplayDate = webFile.DisplayDate.Value.ToTimezone(UserTimeZoneOffset);
            }
            return Auto(webFile);
        }

        // POST: WebFiles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = AllowedFields)] WebFile form, string[] permissions = null)
        {
            WebFile webFile = await WebFile.GetAvailableFiles(db, User, UserManager, RoleManager, !User.IsInRole(Roles.Administrators)).FindAsync(form.Id);
            if (webFile == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                webFile.Update(form, UserTimeZoneOffset);
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
            WebFile webFile = await WebFile.GetAvailableFiles(db, User, UserManager, RoleManager, !User.IsInRole(Roles.Administrators))
                .Include(f => f.Permissions).FindAsync(id);
            if (webFile == null)
            {
                return HttpNotFound();
            }
            var folders = db.ActiveSharedFolders
                .Where(f => f.Files.Select(w => w.Id).Contains(webFile.Id))
                .Include(f => f.Files)
                .ToList();
            foreach (var folder in folders)
            {
                folder.Files.Remove(webFile);
            }
            db.SaveChanges();

            db.WebFiles.Remove(webFile);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
