using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wwwplatform.Models
{
    [Table(name: "SharedFolder")]
    public partial class SharedFolder : Auditable, Permissible
    {
        [Required]
        public string Name { get; set; }

        [MaxLength(150)]
        public string Slug { get; set; }

        public string Description { get; set; }

        public virtual ICollection<WebFile> Files { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }

        public virtual long? ParentFolderId { get; set; }

        [ForeignKey("ParentFolderId")]
        [DisplayName("Parent Folder")]
        public virtual SharedFolder ParentFolder { get; set; }

        [InverseProperty("ParentFolder")]
        public virtual ICollection<SharedFolder> SubFolders { get; set; }
    }
}
