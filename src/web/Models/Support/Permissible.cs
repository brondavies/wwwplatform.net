using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatform.Models
{
    interface Permissible
    {
        ICollection<Permission> Permissions { get; set; }
    }
}
