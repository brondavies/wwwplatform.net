using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static IEnumerable<IdentityResult> CreateAll(ApplicationRoleManager RoleManager)
        {
            if (RoleManager.RoleExists("ListManagers"))
            {
                RoleManager.Delete(RoleManager.Roles.First(r => r.Name == "ListManagers"));
            }
                return new IdentityResult[] {
                CreateRole(Roles.Administrators, RoleManager),
                CreateRole(Roles.Editors, RoleManager),
                CreateRole(Roles.ListManagers, RoleManager),
                CreateRole(Roles.Public, RoleManager),
                CreateRole(Roles.Users, RoleManager)
            };
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
    }
}
