using System;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wwwplatform.Models
{
    public class Auditable
    {
        public Auditable()
        {
            CreatedAt =
            UpdatedAt = DateTime.UtcNow;
        }

        [Key]
        public long Id { get; set; }

        [DisplayName("Updated By")]
        [Display(AutoGenerateField = false)]
        [ScaffoldColumn(false)]
        public virtual string UpdatedBy { get; set; }

        [DisplayName("Updated")]
        [Display(AutoGenerateField = false)]
        [ScaffoldColumn(false)]
        public virtual DateTime UpdatedAt { get; set; }

        [DisplayName("Created")]
        [Display(AutoGenerateField = false)]
        [ScaffoldColumn(false)]
        public virtual DateTime CreatedAt { get; set; }

        [Display(AutoGenerateField = false)]
        [ScaffoldColumn(false)]
        public virtual DateTime? DeletedAt { get; set; }
    }
}
