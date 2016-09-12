using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace wwwplatform.Models.ViewModels
{
    public class ComposeEmailMessageModel
    {
        public string attachments { get; set; }

        [DataType(DataType.Html), AllowHtml]
        public string body { get; set; }

        public string subject { get; set; }
    }
}
