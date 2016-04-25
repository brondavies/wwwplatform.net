using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using wwwplatform.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace wwwplatform.Models
{
    [Table(name: "MailingListSubscriber")]
    public class MailingListSubscriber : Auditable
    {
        public MailingListSubscriber() :base()
        {
            Verification = Extensions.String.Random(64);
        }

        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [DisplayName("Email Address")]
        public string Email { get; set; }

        public string Verification { get; set; }

        [Required]
        public bool Enabled { get; set; }

        public virtual MailingList MailingList { get; set; }
    }
}
