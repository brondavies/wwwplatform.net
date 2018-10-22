using wwwplatform.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace wwwplatform
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                 name: "Pages",
                 url: "{slug}",
                 constraints: new { controller = new SitePageRouteConstraint() },
                 defaults: new { controller = "SitePages", action = "Display" }
             );

            routes.MapRoute(
                 name: "Folders",
                 url: "Shared/{slug}/{action}",
                 defaults: new { controller = "SharedFolders", action = "Display" }
             );

            routes.MapRoute(
                 name: "Files",
                 url: "Downloads/{id}/{extra}",
                 defaults: new { controller = "WebFiles", action = "Details", extra = UrlParameter.Optional }
             );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
