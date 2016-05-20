using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wwwplatform.Models
{
    [Table(name: "EmailAddress")]
    public partial class EmailAddress : Auditable
    {
        public EmailAddress() : base()
        {
            Verification = Extensions.String.Random(64);
        }

        [Required]
        public string Email { get; set; }

        public bool Verified { get; set; }

        public bool VerificationSent { get; set; }

        public string Verification { get; set; }
    }
}
