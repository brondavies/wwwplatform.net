using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace wwwplatform.Extensions.Helpers.Bootstrap
{
    public class BootstrapFormGroup : BootstrapRow
    {
        public BootstrapFormGroup(HtmlHelper htmlHelper, IDictionary<string, object> htmlAttributes) : base(htmlHelper, htmlAttributes)
        {
            className = "form-group";
        }
    }
}
