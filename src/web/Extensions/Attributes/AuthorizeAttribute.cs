using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatform.Extensions.Attributes
{
    public class AuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public AuthorizeAttribute(params string[] Roles) : base()
        {
            base.Roles = string.Join(",", Roles);
        }
    }
}
