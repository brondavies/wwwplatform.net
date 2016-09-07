using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using wwwplatform.Models;

namespace wwwplatform.Extensions.Filters
{
    public class ViewConfigurationFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Controller.ViewBag.Layout == null)
            {
                filterContext.Controller.ViewBag.Layout = Settings.Create(filterContext.HttpContext).DefaultPageLayout;
            }
        }
    }
}