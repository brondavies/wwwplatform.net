using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatform.Models
{
    [Table(name: "MailingList")]
    public partial class MailingList : Auditable
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [DisplayName("List Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAdress { get; set; }

        [DisplayName("List Owner")]
        public ApplicationUser Owner { get; set; }

        public virtual ICollection<MailingListSubscriber> Subscribers { get; set; }
    }
}
