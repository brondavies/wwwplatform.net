using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace wwwplatform.Extensions
{
    public class SitePageRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            string path = httpContext.Request.Url.AbsolutePath;
            string controller = path.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).First();
            var controllers = Assembly.GetExecutingAssembly().DefinedTypes.Where(t => t.Name.EndsWith("Controller") && t.Name.StartsWith(controller));
            return controllers.Count() == 0;
        }
    }
}
