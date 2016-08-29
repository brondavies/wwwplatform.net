using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatform.Models.ViewModels
{
    public class RoleDetailModel
    {
        public IdentityRole Role;
        public IEnumerable<ApplicationUser> Users;
    }
}
