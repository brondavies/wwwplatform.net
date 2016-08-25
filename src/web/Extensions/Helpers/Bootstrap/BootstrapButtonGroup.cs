using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace wwwplatform.Extensions.Helpers.Bootstrap
{
    public class BootstrapButtonGroup : BootstrapElement
    {
        private HtmlHelper _htmlHelper;

        public BootstrapButtonGroup(HtmlHelper htmlHelper, IDictionary<string, object> htmlAttributes) : base(htmlHelper.ViewContext)
        {
            _htmlHelper = htmlHelper;
            _htmlAttributes = htmlAttributes;
            ClassName = "btn-group";
        }
    }
}