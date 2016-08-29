using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using wwwplatform.Extensions;
using wwwplatform.Extensions.Attributes;
using wwwplatform.Models;
using wwwplatform.Models.Serializers;
using wwwplatform.Models.ViewModels;

namespace wwwplatform.Controllers
{
    [Extensions.Authorize(Roles.Administrators)]
    public class RolesController : BaseController
    {
        // GET: Roles
        [HttpGet]
        public ActionResult Index()
        {
            var roles = RoleManager.Roles.ToList();
            return Auto(roles);
        }

        // GET: Roles/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            var roleUserIds = role.Users.Select(ru => ru.UserId);
            var users = db.Users.Where(u => roleUserIds.Contains(u.Id)).ToList();
            return Auto(new RoleDetailModel { Role = role, Users = users });
        }
        
        // POST: Roles/AvailableUsers/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Serializer(typeof(ApplicationUserSerializer))]
        public async Task<ActionResult> AvailableUsers(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            var roleUserIds = role.Users.Select(ru => ru.UserId);
            var available = UserManager.Users.Where(u => !roleUserIds.Contains(u.Id)).ToList();
            return Auto(available);
        }
        
        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FormCollection collection)
        {
            string roleName = collection["roleName"];
            IdentityRole role = new IdentityRole(roleName);
            try
            {
                var result = await RoleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    SetSuccessMessage("Role {0} was added successfully!", role.Name);
                    return RedirectToAction("Details", new { id = role.Id });
                }
                else
                {
                    SetFailureMessage(result.Errors.FirstOrDefault() ?? string.Format("Failed to save the role {0}", roleName));
                }
            }
            catch (Exception e)
            {
                SetFailureMessage(e.Message);
            }
            return RedirectToAction("Index");
        }

        // GET: Roles/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            else if (Roles.IsBuiltinRole(role))
            {
                SetFailureMessage("You cannot change the built-in role {0}", role.Name);
                RedirectToAction("Index");
            }
            return View(role);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, FormCollection collection)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            try
            {
                role.Name = collection["roleName"];
                var result = await RoleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    SetSuccessMessage("Role {0} updated successfully!", role.Name);
                }
                else
                {
                    SetFailureMessage(result.Errors.FirstOrDefault() ?? string.Format("Failed to update the role {0}", role.Name));
                }
                return RedirectToAction("Edit", new { id = role.Id });
            }
            catch
            {
                return View();
            }
        }

        // GET: Roles/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            else if (Roles.IsBuiltinRole(role))
            {
                SetFailureMessage("You cannot delete the built-in role {0}", role.Name);
                RedirectToAction("Index");
            }
            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, FormCollection collection)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            else if (Roles.IsBuiltinRole(role))
            {
                SetFailureMessage("You cannot delete the built-in role {0}", role.Name);
                RedirectToAction("Index");
            }

            try
            {
                var result = await RoleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    SetSuccessMessage("Role {0} deleted successfully!", role.Name);
                }
                else
                {
                    SetFailureMessage(result.Errors.FirstOrDefault() ?? string.Format("Failed to delete the role {0}", role.Name));
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                SetFailureMessage(e.Message);
            }
            return Auto(role);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUsers(string id, string[] userId)
        {
            var success = true;
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            var users = await UserManager.Users.Where(u => userId.Contains(u.Id)).ToListAsync();
            try
            {
                foreach (var user in users)
                {
                    var result = await UserManager.AddToRoleAsync(user.Id, role.Name);
                    if (!result.Succeeded)
                    {
                        success = false;
                        SetFailureMessage(result.Errors.FirstOrDefault() ?? string.Format("Failed to add {0} to role {1}", user.FullName(), role.Name));
                        break;
                    }
                }
            }
            catch(Exception e)
            {
                success = false;
                SetFailureMessage(e.Message);
            }
            if (success)
            {
                string successMessage = "Added {0} to role {1} successfully!";
                string userNames = string.Join(", ", users.Select(u => u.FullName()));
                SetSuccessMessage(successMessage, userNames, role.Name);
            }
            return RedirectToAction("Details", new { id = role.Id });
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveUser(string id, string userId)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            var user = await UserManager.FindByIdAsync(userId);
            try
            {
                var result = await UserManager.RemoveFromRoleAsync(user.Id, role.Name);
                if (result.Succeeded)
                {
                    SetSuccessMessage("{0} removed from role {1} successfully!", user.FullName(), role.Name);
                }
                else
                {
                    SetFailureMessage(result.Errors.FirstOrDefault() ?? string.Format("Failed to remove {0} from role {1}", user.FullName(), role.Name));
                }
            }
            catch
            {
            }
            return RedirectToAction("Details", new { id = role.Id });
        }
    }
}
