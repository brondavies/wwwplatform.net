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
    [Extensions.Attributes.Authorize(Roles.Administrators)]
    public class UsersController : BaseAccountController
    {
        private const string AllowedFields = "ConFirmPassword,Email,EmailConfirmed,FirstName,LastName,LockoutEnabled,LockoutEndDateUtc,Password,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,UserName";

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
            var model = new UserEditorModel();
            var usersRole = RoleManager.Roles.First(r => r.Name == Roles.Users);
            model.permissions = new string[] { usersRole.Id };
            return View(model);
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = AllowedFields)] UserEditorModel editor, string[] permissions)
        {
            try
            {
                if (await CreateUser(editor, permissions, false, false))
                {
                    SetSuccessMessage(string.Format("User {0} was created successfully!", editor.FullName()));
                    return RedirectToAction("Index");
                }
            }
            catch(Exception e)
            {
                SetFailureMessage(e.Message);
            }
            editor.permissions = permissions;
            return View(editor);
        }

        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(new UserEditorModel(user));
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, [Bind(Include = AllowedFields)] UserEditorModel editor, string[] permissions)
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

                ApplyUserRoles(user, permissions);

                var result = await UserManager.UpdateAsync(user);
                if (result == IdentityResult.Success)
                {
                    ModelState.Clear();
                    SetSuccessMessage("User information for {0} was saved successfully!", user.FullName());
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
            }

            editor.permissions = permissions;
            return View(editor);
        }

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            if (user.Id == User.Identity.GetUserId())
            {
                SetFailureMessage("You cannot delete your own user account.");
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, FormCollection collection)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            try
            {
                var result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    SetSuccessMessage("User {0} was deleted successfully!", user.FullName());
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrors(result);
                }
            }
            catch(Exception e)
            {
                SetFailureMessage(e.Message);
            }
            return View(user);
        }
        
        // GET: Users/Edit/5
        public async Task<ActionResult> ResetPassword(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(new AdminResetPasswordViewModel { UserId = user.Id, UserName = user.UserName });
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(AdminResetPasswordViewModel model)
        {
            var user = await UserManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var token = UserManager.GeneratePasswordResetToken(user.Id);
                    var result = await UserManager.ResetPasswordAsync(user.Id, token, model.NewPassword);

                    if (result == IdentityResult.Success)
                    {
                        SetSuccessMessage("Password was updated for {0} successfully!", user.UserName);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw new Exception(string.Join("\r\n", result.Errors));
                    }
                }
                catch (Exception e)
                {
                    SetFailureMessage(e.Message);
                }
            }
            model.UserName = user.UserName;

            return View(model);
        }
    }
}
