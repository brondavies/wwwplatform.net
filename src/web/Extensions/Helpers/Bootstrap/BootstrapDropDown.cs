﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace wwwplatform.Extensions.Helpers.Bootstrap
{
    public class BootstrapDropDown : BootstrapElement
    {
        public string FieldName;
        public object FieldValue;
        public IEnumerable<MenuOption> Options;

        public BootstrapDropDown(ViewContext viewContext, IDictionary<string, object> htmlAttributes = null) : base(viewContext)
        {
            _htmlAttributes = htmlAttributes ?? new Dictionary<string, object>();
            ClassName = "dropdown";
        }

        public override BootstrapElement Start(bool write = true)
        {
            if (base.Start(write) != null)
            {
                var toggleAttributes = new Dictionary<string, object>();
                toggleAttributes["data-toggle"] = "dropdown";
                toggleAttributes["aria-haspopup"] = "true";

                if (!string.IsNullOrEmpty(FieldName))
                {
                    TagBuilder hidden = new TagBuilder("input");
                    hidden.Attributes["type"] = "hidden";
                    hidden.Attributes["name"] = FieldName;
                    hidden.Attributes["value"] = Convert.ToString(FieldValue);
                    _viewContext.Writer.Write(hidden.ToString(TagRenderMode.SelfClosing));
                }

                var toggle = new BootstrapButton(_viewContext, toggleAttributes) { Text = GetValueText(FieldValue) + " " };
                toggle.ClassName += " dropdown-toggle";
                var caret = new BootstrapElement(_viewContext) { TagName = "span", ClassName = "caret" };
                var menu = new BootstrapMenu(_viewContext, null) { Options = Options };

                Add(toggle.Add(caret));
                toggle.Finish();
                Add(menu);
                return this;
            }
            return null;
        }

        private string GetValueText(object value)
        {
            string stringValue = Convert.ToString(value);
            if (Options != null)
            {
                foreach (var option in Options)
                {
                    if (Convert.ToString(option.Value).Equals(stringValue))
                    {
                        return option.Text ?? stringValue;
                    }
                }
            }
            return stringValue;
        }
    }
}