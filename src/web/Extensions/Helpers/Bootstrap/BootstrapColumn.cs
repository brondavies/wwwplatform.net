using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace wwwplatform.Extensions.Helpers.Bootstrap
{
    public class BootstrapColumn : BootstrapElement
    {
        public BootstrapColumn(ViewContext viewContext, IDictionary<string, object> htmlAttributes) : base(viewContext)
        {
            TagName = "div";
            _htmlAttributes = htmlAttributes;
        }

        public BootstrapColumnOffsets ColumnOffset { get; set; }

        public BootstrapColumnSizes ColumnSize { get; set; }

        public override BootstrapColumn Column()
        {
            return Column("div");
        }

        public override BootstrapColumn Column(string tagName = "div")
        {
            return Column(tagName, new { });
        }

        public override BootstrapColumn Column(string tagName = "div", object htmlAttributes = null)
        {
            return Column(tagName, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes ?? new { }));
        }

        public override BootstrapColumn Column(string tagName = "div", IDictionary<string, object> htmlAttributes = null)
        {
            var col = ((BootstrapRow)Parent).Column(tagName, htmlAttributes);
            Finish();
            return col;
        }

        override public BootstrapElement Start(bool write = true)
        {
            if (base.Start(false) != null)
            {
                if (write)
                {
                    TagBuilder tagBuilder = new TagBuilder(TagName);

                    if (_htmlAttributes != null) { tagBuilder.MergeAttributes(_htmlAttributes); }

                    if (ColumnOffset != null)
                    {
                        foreach (var size in ColumnOffset.Keys)
                        {
                            tagBuilder.AddCssClass(string.Format("col-{0}-offset-{1}", size, ColumnOffset[size]));
                        }
                    }

                    if (ColumnSize != null)
                    {
                        foreach (var size in ColumnSize.Keys)
                        {
                            tagBuilder.AddCssClass(string.Format("col-{0}-{1}", size, ColumnSize[size]));
                        }
                    }

                    _viewContext.Writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));
                }
                return null;
            }
            return this;
        }
    }

    public class BootstrapColumnOffsets : BootstrapColumnMeasurements
    {
        public static IEnumerable<BootstrapColumnOffsets> Convert(IEnumerable<object> anonymousObjects)
        {
            return AnonymousObjectConverter.Convert<BootstrapColumnOffsets>(anonymousObjects);
        }
    }

    public class BootstrapColumnSizes : BootstrapColumnMeasurements
    {
        public static IEnumerable<BootstrapColumnSizes> Convert(IEnumerable<object> anonymousObjects)
        {
            return AnonymousObjectConverter.Convert<BootstrapColumnSizes>(anonymousObjects);
        }
    }

    public class BootstrapColumnMeasurements : Dictionary<string, object>
    {
        public int? xs { get { return (int?)this["xs"]; } set { this["xs"] = value.Value; } }
        public int? sm { get { return (int?)this["sm"]; } set { this["sm"] = value.Value; } }
        public int? md { get { return (int?)this["md"]; } set { this["md"] = value.Value; } }
        public int? lg { get { return (int?)this["lg"]; } set { this["lg"] = value.Value; } }
        public int? xl { get { return (int?)this["xl"]; } set { this["xl"] = value.Value; } }
    }

}
