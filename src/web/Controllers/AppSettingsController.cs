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

        // POST: AppSettings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(FormCollection form)
        {
            var settings = db.AppSettings.ToList();
            foreach(var setting in settings)
            {
                if (form.AllKeys.Contains(setting.Name))
                {
                    setting.Value = form[setting.Name];
                    db.Entry(setting).State = EntityState.Modified;
                }
            }
            
            await db.SaveChangesAsync();
            CacheHelper.ClearFromCache(HttpContext, typeof(Settings));

            return RedirectToAction("Index");
        }
    }
}