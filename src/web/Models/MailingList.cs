using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string EmailAddress { get; set; }

        [DisplayName("List Owner")]
        public ApplicationUser Owner { get; set; }

        [DisplayName("Allow Subscribe"), DefaultValue(true)]
        public bool AllowSubscribe { get; set; } = true;

        public virtual ICollection<MailingListSubscriber> Subscribers { get; set; }
    }
}
