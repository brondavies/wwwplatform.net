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

        public static IEnumerable<IdentityResult> CreateAll(ApplicationDbContext context, ApplicationRoleManager roleManager)
        {
            if (roleManager != null && roleManager.RoleExists("ListManagers"))
            {
                roleManager.Delete(roleManager.Roles.First(r => r.Name == "ListManagers"));
            }
            return CreateAll(context, roleManager, Administrators, Editors, ListManagers, Users, Public);
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
                string roleId = context.Database.SqlQuery<string>("SELECT Id FROM dbo.AspNetRoles WHERE Name=@p0", role).FirstOrDefault();
                if (string.IsNullOrEmpty(roleId))
                {
                    context.Database.ExecuteSqlCommand("INSERT dbo.AspNetRoles (Id, Name) VALUES (@p0, @p1)", Guid.NewGuid().ToString(), role);
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
            var roles = new string[] { Roles.Administrators, Roles.Editors, Roles.ListManagers, Roles.Public, Roles.Users };
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
