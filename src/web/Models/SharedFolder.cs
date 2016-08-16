using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wwwplatform.Models
{
    [Table(name: "SharedFolder")]
    public partial class SharedFolder : Auditable, Permissible
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<WebFile> Files { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
