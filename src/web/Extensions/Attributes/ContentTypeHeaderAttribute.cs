using System.Web.Mvc;

namespace wwwplatform.Extensions.Attributes
{
    public class ContentTypeHeaderAttribute : ActionFilterAttribute
    {
        private string ContentType;

        public ContentTypeHeaderAttribute(string contentType) : base()
        {
            ContentType = contentType;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.ContentType = ContentType;
            base.OnActionExecuting(filterContext);
        }
    }
}