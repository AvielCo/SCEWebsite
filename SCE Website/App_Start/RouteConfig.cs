using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SCE_Website
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Login", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "StudentMenu",
            url: "{controller}/{action}",
            defaults: new { controller = "StudentController", action = "Menu" }

            );

            routes.MapRoute(
            name: "LecturerMenu",
            url: "{controller}/{action}",
            defaults: new { controller = "LecturerController", action = "Menu" }

            );

            routes.MapRoute(
            name: "FaAdminMenu",
            url: "{controller}/{action}",
            defaults: new { controller = "FaAdminController", action = "Menu" }

            );
        }
    }
}
