using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wwwplatform.Models
{
    [Table(name: "WebFile")]
    public partial class WebFile : Auditable, Permissible
    {
        private string _filetype;

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public string PreviewLocation { get; set; }

        public long? Size { get; set; }

        [DisplayName("Display Date")]
        public DateTime? DisplayDate { get; set; }

        public string Filetype { get { return _filetype ?? GetFileType().ToString(); } set { _filetype = value; } }

        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
