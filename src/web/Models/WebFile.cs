using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wwwplatform.Models
{
    [Table(name: "WebFile")]
    public partial class WebFile : Auditable, Permissible
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public long? Size { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
