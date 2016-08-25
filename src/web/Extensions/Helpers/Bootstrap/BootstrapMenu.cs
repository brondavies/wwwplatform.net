using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace wwwplatform.Extensions.Helpers.Bootstrap
{
    public class BootstrapMenu : BootstrapElement
    {
        public dynamic[] Options;

        public BootstrapMenu(ViewContext viewContext, IDictionary<string, object> htmlAttributes) : base(viewContext)
        {
            TagName = "ul";
            ClassName = "dropdown-menu";
        }

        public override BootstrapElement Start(bool write = true)
        {
            if (null != base.Start(write))
            {
                if (Options != null)
                {
                    TagBuilder b = new TagBuilder("a");
                    b.Attributes["href"] = "#";

                    foreach (dynamic option in Options)
                    {
                        var element = new BootstrapElement(_viewContext) { TagName = "li" };
                        b.SetInnerText(option.Text);
                        b.Attributes["data-value"] = option.Value ?? option.Text;
                        Add(element.Add(b));
                    }
                }
                return this;
            }
            return null;
        }
    }
}
