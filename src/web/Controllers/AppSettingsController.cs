using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using wwwplatform.Extensions;
using wwwplatform.Extensions.Helpers;
using wwwplatform.Models;

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
            var settings = db.AppSettings.ToList();
            foreach(var setting in settings)
            {
                if (form.AllKeys.Contains(setting.Name))
                {
                    var normalizedValue = NullIfEmpty(form[setting.Name]);
                    if (!string.IsNullOrEmpty(normalizedValue))
                    if (setting.Kind == AppSetting.KindDirectory)
                    {
                        if (!ExistsDir(normalizedValue))
                        {
                            SetFailureMessage(normalizedValue + " does not exist.");
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
                    }
                    setting.Value = normalizedValue;
                    db.Entry(setting).State = EntityState.Modified;
                }
            }
            
            await db.SaveChangesAsync();
            CacheHelper.ClearFromCache(HttpContext, typeof(Settings));
            SetSuccessMessage("Settings updated successfully!");

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
            try {
                exists = System.IO.Directory.Exists(Server.MapPath(normalizedValue));
            } catch { }
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