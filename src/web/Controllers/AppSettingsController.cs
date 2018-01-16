using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using wwwplatform.Extensions;
using wwwplatform.Extensions.Helpers;
using wwwplatform.Models;
using wwwplatform.Models.Support;
using wwwplatform.Shared.Helpers;

namespace wwwplatform.Controllers
{
    [Extensions.Authorize(Roles.Administrators)]
    public class AppSettingsController : BaseController
    {
        // GET: AppSettings
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var settings = await db.AppSettings.OrderBy(m => m.Name).ToListAsync();
            return View(settings);
        }

        // POST: AppSettings/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(FormCollection form)
        {
            bool require_restart = false;
            var settings = await db.AppSettings.ToListAsync();
            foreach (var setting in settings)
            {
                if (form.AllKeys.Contains(setting.Name))
                {
                    var normalizedValue = NullIfEmpty(form[setting.Name]);
                    if (!string.IsNullOrEmpty(normalizedValue))
                    {
                        if (setting.Kind == AppSetting.KindDirectory)
                        {
                            if (!ExistsDir(normalizedValue))
                            {
                                SetFailureMessage(normalizedValue + " does not exist.");
                                continue;
                            }
                        }
                        else
                        if (setting.Kind == AppSetting.KindNumber)
                        {
                            double doubleResult;
                            if (!double.TryParse(normalizedValue, out doubleResult))
                            {
                                SetFailureMessage(normalizedValue + string.Format(" is not a valid value for {0}.", setting.Name));
                                continue;
                            }
                        }
                        else
                        if (setting.Kind == AppSetting.KindFile)
                        {
                            if (!ExistsFile(normalizedValue))
                            {
                                SetFailureMessage(normalizedValue + " does not exist.");
                                continue;
                            }
                            else
                            if (setting.Name == Settings.kSkinDefinitionFile && setting.Value != normalizedValue)
                            {
                                try
                                {
                                    SkinDefinition skindef = SkinDefinition.Load(HttpContext.Server.MapPath(normalizedValue));
                                    settings.Find(s => s.Name == Settings.kDefaultPageLayout).Value = skindef.layout;
                                }
                                catch
                                {
                                    SetFailureMessage(normalizedValue + " is not a valid skin definition file.");
                                    continue;
                                }
                            }
                        }
                        if (setting.Name == Settings.kCreatePDFVersionsOfDocuments && normalizedValue == bool.TrueString)
                        {
                            if (!System.IO.File.Exists(Settings.ConvertPdfExe))
                            {
                                SetFailureMessage("Cannot enable PDF conversion because ConvertPdf.exe is missing.");
                                continue;
                            }
                            else
                            {
                                int exitcode = DocumentHelper.CheckConverterSetup(Settings.ConvertPdfExe, System.IO.Path.Combine(Settings.TempDir, "convertpdf.log"));
                                if (exitcode < 0)
                                {
                                    SetFailureMessage("(Error:{0}) Cannot enable PDF conversion because ConvertPdf.exe failed to run.", exitcode);
                                    continue;
                                }
                                else if ((exitcode & 7) != 7)
                                {
                                        SetFailureMessage("(Error:{0}) Cannot enable PDF conversion because the Microsoft Office installation is missing or incomplete.", exitcode);
                                        continue;
                                }
                            }
                        }
                    }
                    if (setting.Name == Settings.kSkinDefinitionFile && setting.Value != normalizedValue)
                    {
                        require_restart = true;
                    }
                    setting.Value = normalizedValue;
                    db.Entry(setting).State = EntityState.Modified;
                }
            }

            await db.SaveChangesAsync();
            CacheHelper.ClearFromCache(HttpContext, typeof(Settings));
            SetSuccessMessage("Settings updated successfully!");

            if (require_restart)
            {
                HttpRuntime.UnloadAppDomain();
            }

            return RedirectToAction("Index");
        }

        private bool ExistsFile(string normalizedValue)
        {
            var exists = false;
            try
            {
                exists = System.IO.File.Exists(Server.MapPath(normalizedValue));
            }
            catch { }
            return exists;
        }

        private bool ExistsDir(string normalizedValue)
        {
            var exists = false;
            try
            {
                exists = System.IO.Directory.Exists(Server.MapPath(normalizedValue));
            }
            catch { }
            return exists;
        }

        private string NullIfEmpty(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            return value.Trim();
        }
    }
}