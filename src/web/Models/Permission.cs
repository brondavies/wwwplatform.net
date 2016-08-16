using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace wwwplatform.Models
{
    public partial class Permission
    {
        public Permission()
        {
            Grant = true;
        }

        [Key, Column(Order = 0)]
        public long Id { get; set; }
        
        [DisplayName("Updated By")]
        [Display(AutoGenerateField = false)]
        [ScaffoldColumn(false)]
        public virtual string UpdatedBy { get; set; }

        [DefaultValue(true)]
        public bool Grant { get; set; }

        [DefaultValue(false)]
        public bool Deny { get; set; }

        [DisplayName("Applies to Role")]
        [ForeignKey("AppliesToRole_Id")]
        public virtual IdentityRole AppliesToRole { get; set; }

        public virtual string AppliesToRole_Id { get; set; }
        
        [DisplayName("Applies to User")]
        [ForeignKey("AppliesTo_Id")]
        public virtual ApplicationUser AppliesTo { get; set; }

        public virtual string AppliesTo_Id { get; set; }
    }
}
