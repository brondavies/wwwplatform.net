using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wwwplatform.Models
{
    public partial class MailingListSubscriber : Auditable
    {
        public void Update(MailingListSubscriber values)
        {
            Email = values.Email;
            Enabled = values.Enabled;
            FirstName = values.FirstName;
            LastName = values.LastName;
        }

        public string FullName()
        {
            return (FirstName + " " + LastName).Trim();
        }
    }
}