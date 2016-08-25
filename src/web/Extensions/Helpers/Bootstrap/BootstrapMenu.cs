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
        public IEnumerable<MenuOption> Options;

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
                    foreach (var option in Options)
                    {
                        var element = new BootstrapElement(_viewContext) { TagName = "li" };
                        TagBuilder b = new TagBuilder("a");
                        b.Attributes["href"] = "#";
                        b.Attributes["data-value"] = (option.Value != null) ? Convert.ToString(option.Value) : option.Text;
                        b.SetInnerText(option.Text ?? Convert.ToString(option.Value));
                        Add(element.Add(b));
                    }
                }
                return this;
            }
            return null;
        }
    }
}
