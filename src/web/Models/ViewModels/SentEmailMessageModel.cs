using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatform.Models
{
    public class SentEmailMessageModel
    {
        [DisplayName("Number of Recipients")]
        public int SentCount { get; set; }

        [DisplayName("Number of Subscribers")]
        public int TotalCount { get; set; }

        public long MailingListId { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public bool Success { get; set; }
    }
}
