using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace wwwplatform.Models
{
    [Table(name: "EmailMessage")]
    public class EmailMessage : Auditable
    {
        [Required]
        public string Subject { get; set; }

        public string To { get; set; }

        public string BCC { get; set; }

        public string CC { get; set; }

        [Required]
        [DataType(DataType.Html), UIHint("tinymce_jquery_full"), AllowHtml]
        public string Body { get; set; }

        public bool IsHtml { get; set; }

        public DateTime? SentDate { get; set; }

        public virtual MailingList MailingList { get; set; }

        public virtual ICollection<WebFile> Attachments { get; set; }
    }
}
