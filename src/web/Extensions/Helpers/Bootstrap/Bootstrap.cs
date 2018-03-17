using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace wwwplatform.Extensions.Helpers.Bootstrap
{
    public static class Bootstrap
    {
        #region BootstrapRow

        public static BootstrapRow Row(this HtmlHelper htmlHelper, IEnumerable<object> columnSizes = null)
        {
            return RowHelper(htmlHelper, new RouteValueDictionary(), BootstrapColumnSizes.Convert(columnSizes), null);
        }

        public static BootstrapRow Row(this HtmlHelper htmlHelper, IEnumerable<object> columnSizes, IEnumerable<object> columnOffsets = null)
        {
            return RowHelper(htmlHelper, new RouteValueDictionary(), BootstrapColumnSizes.Convert(columnSizes), BootstrapColumnOffsets.Convert(columnOffsets));
        }

        public static BootstrapRow Row(this HtmlHelper htmlHelper, object htmlAttributes, IEnumerable<object> columnSizes, IEnumerable<object> columnOffsets = null)
        {
            return RowHelper(htmlHelper, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), BootstrapColumnSizes.Convert(columnSizes), BootstrapColumnOffsets.Convert(columnOffsets));
        }

        public static BootstrapRow Row(this HtmlHelper htmlHelper, IDictionary<string, object> htmlAttributes, IEnumerable<object> columnSizes, IEnumerable<object> columnOffsets = null)
        {
            return RowHelper(htmlHelper, htmlAttributes, BootstrapColumnSizes.Convert(columnSizes), BootstrapColumnOffsets.Convert(columnOffsets));
        }

        public static BootstrapRow Row(this HtmlHelper htmlHelper, BootstrapColumnSizes[] columnSizes, BootstrapColumnOffsets[] columnOffsets)
        {
            return RowHelper(htmlHelper, new RouteValueDictionary(), columnSizes, columnOffsets);
        }

        public static BootstrapRow Row(this HtmlHelper htmlHelper, object htmlAttributes, BootstrapColumnSizes[] columnSizes, BootstrapColumnOffsets[] columnOffsets)
        {
            return RowHelper(htmlHelper, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), columnSizes, columnOffsets);
        }

        public static BootstrapRow Row(this HtmlHelper htmlHelper, IDictionary<string, object> htmlAttributes, BootstrapColumnSizes[] columnSizes, BootstrapColumnOffsets[] columnOffsets)
        {
            return RowHelper(htmlHelper, htmlAttributes, columnSizes, columnOffsets);
        }

        private static BootstrapRow RowHelper(this HtmlHelper htmlHelper, IDictionary<string, object> htmlAttributes, IEnumerable<BootstrapColumnSizes> columnSizes, IEnumerable<BootstrapColumnOffsets> columnOffsets)
        {
            BootstrapRow row = new BootstrapRow(htmlHelper, htmlAttributes);
            if (columnSizes != null)
            {
                row.ColumnSizes = columnSizes.ToArray();
            }

            if (columnOffsets != null)
            {
                row.ColumnOffsets = columnOffsets.ToArray();
            }

            return row;
        }

        #endregion

        #region BootstrapFormGroup

        public static BootstrapFormGroup FormGroup(this HtmlHelper htmlHelper)
        {
            return FormGroupHelper(htmlHelper, new RouteValueDictionary(), null, null);
        }

        public static BootstrapFormGroup FormGroup(this HtmlHelper htmlHelper, IEnumerable<object> columnSizes)
        {
            return FormGroupHelper(htmlHelper, new RouteValueDictionary(), BootstrapColumnSizes.Convert(columnSizes), null);
        }

        public static BootstrapFormGroup FormGroup(this HtmlHelper htmlHelper, IEnumerable<object> columnSizes, IEnumerable<object> columnOffsets)
        {
            return FormGroupHelper(htmlHelper, new RouteValueDictionary(), BootstrapColumnSizes.Convert(columnSizes), BootstrapColumnOffsets.Convert(columnOffsets));
        }

        public static BootstrapFormGroup FormGroup(this HtmlHelper htmlHelper, object htmlAttributes, IEnumerable<object> columnSizes, IEnumerable<object> columnOffsets = null)
        {
            return FormGroupHelper(htmlHelper, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), BootstrapColumnSizes.Convert(columnSizes), BootstrapColumnOffsets.Convert(columnOffsets));
        }

        public static BootstrapFormGroup FormGroup(this HtmlHelper htmlHelper, IDictionary<string, object> htmlAttributes, IEnumerable<object> columnSizes, IEnumerable<object> columnOffsets = null)
        {
            return FormGroupHelper(htmlHelper, htmlAttributes, BootstrapColumnSizes.Convert(columnSizes), BootstrapColumnOffsets.Convert(columnOffsets));
        }

        public static BootstrapFormGroup FormGroup(this HtmlHelper htmlHelper, BootstrapColumnSizes[] columnSizes, BootstrapColumnOffsets[] columnOffsets)
        {
            return FormGroupHelper(htmlHelper, new RouteValueDictionary(), columnSizes, columnOffsets);
        }

        public static BootstrapFormGroup FormGroup(this HtmlHelper htmlHelper, object htmlAttributes, BootstrapColumnSizes[] columnSizes, BootstrapColumnOffsets[] columnOffsets)
        {
            return FormGroupHelper(htmlHelper, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), columnSizes, columnOffsets);
        }

        public static BootstrapFormGroup FormGroup(this HtmlHelper htmlHelper, IDictionary<string, object> htmlAttributes, BootstrapColumnSizes[] columnSizes, BootstrapColumnOffsets[] columnOffsets)
        {
            return FormGroupHelper(htmlHelper, htmlAttributes, columnSizes, columnOffsets);
        }

        private static BootstrapFormGroup FormGroupHelper(this HtmlHelper htmlHelper, IDictionary<string, object> htmlAttributes, IEnumerable<BootstrapColumnSizes> columnSizes, IEnumerable<BootstrapColumnOffsets> columnOffsets)
        {
            BootstrapFormGroup formgroup = new BootstrapFormGroup(htmlHelper, htmlAttributes);
            if (columnSizes != null)
            {
                formgroup.ColumnSizes = columnSizes.ToArray();
            }

            if (columnOffsets != null)
            {
                formgroup.ColumnOffsets = columnOffsets.ToArray();
            }

            return formgroup;
        }

        #endregion

        #region BootstrapColumn

        public static BootstrapColumn Column(this HtmlHelper htmlHelper)
        {
            return ColumnHelper(htmlHelper, null, new RouteValueDictionary());
        }

        public static BootstrapColumn Column(this HtmlHelper htmlHelper, string tagName)
        {
            return ColumnHelper(htmlHelper, tagName, new RouteValueDictionary());
        }

        public static BootstrapColumn Column(this HtmlHelper htmlHelper, string tagName, object htmlAttributes)
        {
            return ColumnHelper(htmlHelper, tagName, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static BootstrapColumn Column(this HtmlHelper htmlHelper, string tagName, IDictionary<string, object> htmlAttributes)
        {
            return ColumnHelper(htmlHelper, tagName, htmlAttributes);
        }

        public static BootstrapColumn Column(this HtmlHelper htmlHelper, object htmlAttributes)
        {
            return ColumnHelper(htmlHelper, null, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static BootstrapColumn Column(this HtmlHelper htmlHelper, IDictionary<string, object> htmlAttributes)
        {
            return ColumnHelper(htmlHelper, null, htmlAttributes);
        }

        private static BootstrapColumn ColumnHelper(this HtmlHelper htmlHelper, string tagName, IDictionary<string, object> htmlAttributes)
        {
            BootstrapColumn Column = new BootstrapColumn(htmlHelper.ViewContext, htmlAttributes);
            Column.TagName = tagName ?? "div";

            return Column;
        }

        #endregion

        #region ControlLabel
        
        public static MvcHtmlString ControlLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            return ControlLabelHelper(html,
                               ModelMetadata.FromLambdaExpression(expression, html.ViewData),
                               ExpressionHelper.GetExpressionText(expression));
        }

        public static MvcHtmlString ControlLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText)
        {
            return ControlLabelHelper(html,
                               ModelMetadata.FromLambdaExpression(expression, html.ViewData),
                               ExpressionHelper.GetExpressionText(expression),
                               labelText);
        }

        public static MvcHtmlString ControlLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return ControlLabelHelper(html,
                               ModelMetadata.FromLambdaExpression(expression, html.ViewData),
                               ExpressionHelper.GetExpressionText(expression),
                               null,
                               HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ControlLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            return ControlLabelHelper(html,
                               ModelMetadata.FromLambdaExpression(expression, html.ViewData),
                               ExpressionHelper.GetExpressionText(expression),
                               null,
                               htmlAttributes);
        }

        public static MvcHtmlString ControlLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText, object htmlAttributes)
        {
            return ControlLabelHelper(html,
                               ModelMetadata.FromLambdaExpression(expression, html.ViewData),
                               ExpressionHelper.GetExpressionText(expression),
                               labelText,
                               HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ControlLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText, IDictionary<string, object> htmlAttributes)
        {
            return ControlLabelHelper(html,
                               ModelMetadata.FromLambdaExpression(expression, html.ViewData),
                               ExpressionHelper.GetExpressionText(expression),
                               labelText,
                               htmlAttributes);
        }
        
        internal static MvcHtmlString ControlLabelHelper(HtmlHelper html, ModelMetadata metadata, string htmlFieldName, string labelText = null, IDictionary<string, object> htmlAttributes = null)
        {
            string resolvedLabelText = labelText ?? metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (string.IsNullOrEmpty(resolvedLabelText))
            {
                return MvcHtmlString.Empty;
            }

            TagBuilder tag = new TagBuilder("label");
            tag.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)));
            tag.SetInnerText(resolvedLabelText);
            tag.MergeAttributes(htmlAttributes, replaceExisting: true);
            tag.AddCssClass("control-label");
            return new MvcHtmlString(tag.ToString());
        }

        #endregion

        #region FormControl

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var className = typeof(TValue) == typeof(bool) ? "" : "form-control";
            return FormControlHelper(html, expression, new { htmlAttributes = new { @class = className } });
        }

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, dynamic additionalViewData)
        {
            if (additionalViewData != null)
            {
                var className = typeof(TValue) == typeof(bool) ? "" : "form-control";
                if (className != "")
                {
                    if (additionalViewData.htmlAttributes != null)
                    {
                        if (additionalViewData.htmlAttributes.@class == null)
                        {
                            additionalViewData.htmlAttributes.@class = className;
                        }
                        else
                        {
                            additionalViewData.htmlAttributes.@class += " " + className;
                        }
                    }
                    else
                    {
                        additionalViewData.htmlAttributes = new { @class = className };
                    }
                }
            }
            return FormControlHelper(html, expression, additionalViewData);
        }

        public static MvcHtmlString FormControlHelper<TModel, TValue>(HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object additionalViewData)
        {
            return html.EditorFor(expression, additionalViewData).Concat(
                html.ValidationMessageFor(expression, "", new { @class = "text-danger" }));
        }

        #endregion

        #region BootstrapDropDown

        public static MvcHtmlString FormDropDown(this HtmlHelper html, FormDropDownOptions options, dynamic additionalViewData = null)
        {
            return DropDownHelper(html, HtmlHelper.AnonymousObjectToHtmlAttributes(additionalViewData), options).ToMvcHtmlString();
        }

        private static BootstrapDropDown DropDownHelper(this HtmlHelper htmlHelper, IDictionary<string, object> htmlAttributes, FormDropDownOptions options)
        {
            BootstrapDropDown dropdown = new BootstrapDropDown(htmlHelper.ViewContext, htmlAttributes);
            if (options != null)
            {
                dropdown.FieldName = options.Name;
                dropdown.FieldValue = options.Value;
                dropdown.Options = options.Options;
            }
            
            return dropdown;
        }
        #endregion

        public static MvcHtmlString Concat(this MvcHtmlString first, params MvcHtmlString[] strings)
        {
            return MvcHtmlString.Create(first.ToString() + string.Concat(strings.Select(s => s.ToString())));
        }

        internal static void EndElement(ViewContext viewContext, string tagName)
        {
            viewContext.Writer.Write("</" + tagName + ">");
        }
    }
}
