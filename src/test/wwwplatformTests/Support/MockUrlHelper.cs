using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace wwwplatformTests.Support
{
    public class MockUrlHelper : UrlHelper
    {
        public Func<string, string, RouteValueDictionary, string, string> ActionCallback;

        public MockUrlHelper(RequestContext requestContext) : base(requestContext)
        {
        }

        public override string Action(string actionName, string controllerName = null, object routeValues = null, string protocol = null)
        {
            if (ActionCallback == null)
            {
                return base.Action(actionName, controllerName, routeValues, protocol);
            }
            RouteValueDictionary dict = new RouteValueDictionary(routeValues);
            return ActionCallback.Invoke(actionName, controllerName, dict, protocol);
        }
    }
}
