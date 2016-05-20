using wwwplatform.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Data.Entity;
using wwwplatform.Models;
using wwwplatform.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace wwwplatform.Controllers
{
    [Extensions.Authorize(Roles.Administrators)]
    public class UsersController : BaseController
    {
        private const string AllowedFields = "Email,EmailConfirmed,FirstName,LastName,LockoutEnabled,LockoutEndDateUtc,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled";

        // GET: Users
        public async Task<ActionResult> Index()
        {
            var allusers = await db.Users.ToListAsync();
            return View(allusers);
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(string id, [Bind(Include = AllowedFields)] UserEditorModel editor)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            try
            {
                user.Email = editor.Email;
                user.EmailConfirmed = editor.EmailConfirmed;
                user.FirstName = editor.FirstName;
                user.LastName = editor.LastName;
                user.LockoutEnabled = editor.LockoutEnabled;
                user.LockoutEndDateUtc = editor.LockoutEndDateUtc;
                user.PhoneNumber = editor.PhoneNumber;
                user.PhoneNumberConfirmed = editor.PhoneNumberConfirmed;
                user.TwoFactorEnabled = editor.TwoFactorEnabled;
                //not allowed user.UserName = editor.UserName;

                var result = await UserManager.UpdateAsync(user);
                if (result == IdentityResult.Success)
                {
                    SetSuccessMessage(string.Format("User information for {0} was saved successfully!", user.FullName()));
                    return RedirectToAction("Index");
                }
                else
                {
                    throw new Exception(string.Join("\r\n", result.Errors));
                }
            }
            catch(Exception e)
            {
                SetFailureMessage(e.Message);
                return View(user);
            }
        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            return View();
        }

        // POST: Users/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
