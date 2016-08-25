using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace wwwplatform.Extensions.Helpers.Bootstrap
{
    public class BootstrapButton : BootstrapElement
    {
        public BootstrapButton(ViewContext viewContext, IDictionary<string, object> htmlAttributes) : base(viewContext)
        {
            _htmlAttributes = htmlAttributes;
            ClassName = "btn";
            TagName = "button";
        }
    }
}