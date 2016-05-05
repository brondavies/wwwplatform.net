using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatform.Models
{
    public class Permission
    {
        public Permission()
        {
            Grant = true;
        }

        [Key]
        public long Id { get; set; }

        [DisplayName("Updated By")]
        [Display(AutoGenerateField = false)]
        [ScaffoldColumn(false)]
        public virtual string UpdatedBy { get; set; }

        [DefaultValue(true)]
        public bool Grant { get; set; }

        [DefaultValue(false)]
        public bool Deny { get; set; }

        public long ContentId { get; set; }
        
        [DisplayName("Applies to Role")]
        public virtual IdentityRole AppliesToRole { get; set; }
        
        [DisplayName("Applies to User")]
        public virtual ApplicationUser AppliesTo { get; set; }
    }
}
