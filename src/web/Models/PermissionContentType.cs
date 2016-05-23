using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatform.Models
{
    public enum PermissionContentType
    {
        File = 1,
        List = 2,
        Page = 3
        //TODO: Update Permission.ContentType Validator if other content types are added
    }
}
