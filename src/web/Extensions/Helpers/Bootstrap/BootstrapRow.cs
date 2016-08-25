using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace wwwplatform.Extensions.Helpers.Bootstrap
{
    public class BootstrapRow : BootstrapElement
    {
        private HtmlHelper _htmlHelper;
        private int _columns;

        private BootstrapColumnSizes[] _columnSizes;

        public BootstrapColumnSizes[] ColumnSizes { get { return _columnSizes; } set { _columnSizes = value; } }

        private BootstrapColumnOffsets[] _columnOffsets;

        public BootstrapColumnOffsets[] ColumnOffsets { get { return _columnOffsets; } set { _columnOffsets = value; } }
        
        public BootstrapRow(HtmlHelper htmlHelper, IDictionary<string, object> htmlAttributes) : base(htmlHelper.ViewContext)
        {
            _columns = 0;
            _htmlHelper = htmlHelper;
            _htmlAttributes = htmlAttributes;
            ClassName = "row";
        }

        public override BootstrapColumn Column()
        {
            return Column(null, null);
        }

        public override BootstrapColumn Column(string tagName = "div")
        {
            return Column(tagName, null);
        }

        public override BootstrapColumn Column(string tagName = "div", object htmlAttributes = null)
        {
            return Column(tagName, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes ?? new { }));
        }

        public override BootstrapColumn Column(string tagName = "div", IDictionary<string, object> htmlAttributes = null)
        {
            this.Start();
            var column = new BootstrapColumn(_htmlHelper.ViewContext, htmlAttributes ?? new ViewDataDictionary());
            column.TagName = tagName;
            column.ColumnOffset = NextOffset();
            column.ColumnSize = NextSize();
            _columns++;
            column.Parent = this;
            
            return column;
        }

        internal BootstrapColumnOffsets NextOffset()
        {
            if (_columnOffsets != null)
            {
                if (_columns >= _columnOffsets.Length)
                {
                    return null;
                }
                var col = _columnOffsets[_columns];
                return col;
            }
            return null;
        }

        internal BootstrapColumnSizes NextSize()
        {
            if (_columnSizes != null)
            {
                if (_columns >= _columnSizes.Length)
                {
                    throw new IndexOutOfRangeException("There are not enough columns specified");
                }
                var col = _columnSizes[_columns];
                return col;
            }
            return null;
        }
    }
}
