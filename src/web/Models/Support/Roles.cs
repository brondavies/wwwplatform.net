using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatform.Models
{
    public class Roles
    {
        public const string Administrators = "Administrators";
        public const string Editors = "Editors";
        public const string ListManagers = "List Managers";
        public const string Users = "Users";
        public const string Public = "Public";
        public const string Guests = "Guests";

        public static IEnumerable<IdentityResult> CreateAll(ApplicationDbContext context, ApplicationRoleManager roleManager)
        {
            if (roleManager != null && roleManager.RoleExists("ListManagers"))
            {
                roleManager.Delete(roleManager.Roles.First(r => r.Name == "ListManagers"));
            }
            return CreateAll(context, roleManager, Administrators, Editors, ListManagers, Users, Public, Guests);
        }

        private static IEnumerable<IdentityResult> CreateAll(ApplicationDbContext context, ApplicationRoleManager roleManager, params string[] roles)
        {
            IdentityResult[] created = new IdentityResult[roles.Length];
            int index = 0;
            foreach (var role in roles)
            {
                created[index] = (roleManager != null) ? CreateRole(role, roleManager) : CreateRole(role, context);
                index++;
            }
            return created;
        }

        private static IdentityResult CreateRole(string role, ApplicationDbContext context)
        {
            try
            {
                IdentityRole existing = context.Roles.Where(r => r.Name == role).FirstOrDefault();
                if (existing == null)
                {
                    context.Roles.Add(new IdentityRole { Id = Guid.NewGuid().ToString(), Name = role });
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                return new IdentityResult(e.Message);
            }
            return null;
        }

        private static IdentityResult CreateRole(string roleName, ApplicationRoleManager RoleManager)
        {
            if (!RoleManager.RoleExists(roleName))
            {
                var identresult = RoleManager.Create(new IdentityRole
                {
                    Name = roleName
                });
                if (!identresult.Succeeded)
                {
                    return identresult;
                }
            }
            return null;
        }

        public static bool IsBuiltinRole(IdentityRole role)
        {
            var roles = new string[] { Roles.Administrators, Roles.Editors, Roles.ListManagers, Roles.Public, Roles.Users, Roles.Guests };
            return (roles.Contains(role.Name));
        }

        public static bool UserInAnyRole(IPrincipal User, ApplicationRoleManager RoleManager, params string[] roleIds)
        {
            if (!User.Identity.IsAuthenticated) { return false; }
            if (User.IsInRole(Roles.Administrators)) { return true; }
            var userRoles = RoleManager.Roles.Where(r => roleIds.Contains(r.Id)).ToList();
            foreach (var r in userRoles)
            {
                if (User.IsInRole(r.Name)) return true;
            }
            return false;
        }

        public static bool UserInAnyRole(IPrincipal User, params string[] roleNames)
        {
            if (!User.Identity.IsAuthenticated) { return false; }
            if (User.IsInRole(Roles.Administrators)) { return true; }
            foreach (var r in roleNames)
            {
                if (User.IsInRole(r)) return true;
            }
            return false;
        }
    }
}
