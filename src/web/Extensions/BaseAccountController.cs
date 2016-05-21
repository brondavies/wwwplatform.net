using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using wwwplatform.Models;
using System.Web.Mvc;

namespace wwwplatform.Extensions
{
    public class BaseAccountController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            set
            {
                _signInManager = value;
            }
        }

        protected async Task<bool> CreateUser(RegisterViewModel model, string[] roles, bool andSignIn, bool andConfirmEmail)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (ModelState.IsValid)
                    {
                        ApplyUserRoles(user, roles);

                        if (andConfirmEmail)
                        {
                            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            await UserManager.SendEmailAsync(user.Id, "Confirm your account", "<a href=\"" + callbackUrl + "\">Please confirm your account by clicking here</a>");
                        }

                        if (andSignIn)
                        {
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        }

                        return true;
                    }
                }
                else
                {
                    AddErrors(result);
                }
            }

            return ModelState.IsValid;
        }

        protected void ApplyUserRoles(ApplicationUser user, string[] roles)
        {
            var existingRoles = user.Roles.ToList();
            foreach(var role in existingRoles)
            {
                if (roles==null || !roles.Any(r => r == role.RoleId))
                {
                    UserManager.RemoveFromRole(user.Id, RoleManager.FindById(role.RoleId).Name);
                }
            }

            if (roles != null)
            {
                foreach (string role in roles)
                {
                    if (RoleManager.Roles.Any(r => r.Id == role) && !UserManager.IsInRole(user.Id, role))
                    {
                        UserManager.AddToRole(user.Id, RoleManager.FindById(role).Name);
                    }
                }
            }
        }

        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
