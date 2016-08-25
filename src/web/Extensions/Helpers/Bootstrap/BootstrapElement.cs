using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebPages;

namespace wwwplatform.Extensions.Helpers.Bootstrap
{
    public class BootstrapElement : IDisposable
    {
        private bool _disposed;
        private string _tagName;
        private BootstrapElement _parent;

        protected IDictionary<string, object> _htmlAttributes;
        protected ViewContext _viewContext;
        protected int _children;

        public virtual BootstrapElement Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != value)
                {
                    _parent = value;
                    _parent._children++;
                }
            }
        }

        public string TagName { get { return _tagName; } set { _tagName = value ?? _tagName; } }

        public string ClassName { get; set; }

        public string Text { get; set; }

        public BootstrapElement(ViewContext viewContext)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException("viewContext");
            }

            _viewContext = viewContext;
            TagName = "div";
            _children = 0;
        }

        
        public virtual BootstrapElement Add(Func<dynamic, HelperResult> expression)
        {
            return Add(expression.Invoke(null));
        }

        public virtual BootstrapElement Add(object value)
        {
            Start();
            _viewContext.Writer.Write(value);
            return this;
        }

        public virtual BootstrapElement Add(HelperResult element)
        {
            Start();
            element.WriteTo(_viewContext.Writer);
            return this;
        }

        public virtual BootstrapElement Add(MvcHtmlString element)
        {
            Start();
            _viewContext.Writer.Write(element);
            return this;
        }

        public virtual BootstrapColumn Column()
        {
            throw new NotImplementedException();
        }

        public virtual BootstrapColumn Column(string tagName = "div")
        {
            throw new NotImplementedException();
        }

        public virtual BootstrapColumn Column(string tagName = "div", object htmlAttributes = null)
        {
            throw new NotImplementedException();
        }

        public virtual BootstrapColumn Column(string tagName = "div", IDictionary<string, object> htmlAttributes = null)
        {
            throw new NotImplementedException();
        }

        private bool _started;
        public virtual BootstrapElement Start(bool write = true)
        {
            if (_started) { return null; }
            if (Parent != null)
            {
                Parent.Start();
            }
            _started = true;

            if (write)
            {
                TagBuilder tagBuilder = new TagBuilder(TagName);
                if (_htmlAttributes != null) { tagBuilder.MergeAttributes(_htmlAttributes); }
                if (!string.IsNullOrWhiteSpace(ClassName))
                {
                    tagBuilder.AddCssClass(ClassName);
                }

                if (Text != null)
                {
                    tagBuilder.SetInnerText(Text);
                }
                _viewContext.Writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));
                _viewContext.Writer.Write(tagBuilder.InnerHtml);
            }

            return this;
        }

        public virtual MvcHtmlString Finish()
        {
            _children--;
            if (_children > 0)
            {
                return null;
            }
            return ToMvcHtmlString();
        }

        public override string ToString()
        {
            Start();
            Dispose(true);
            if (Parent != null)
            {
                Parent.Finish();
            }
            return string.Empty;
        }

        public virtual MvcHtmlString ToMvcHtmlString()
        {
            return new MvcHtmlString(ToString());
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                Bootstrap.EndElement(_viewContext, TagName);
            }
        }
    }
}
