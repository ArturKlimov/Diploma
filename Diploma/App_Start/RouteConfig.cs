using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Diploma
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "admin",
                url: "admin",
                defaults: new { controller = "Admin", action = "Index" }
            );

            routes.MapRoute(
                name: "news page",
                url: "news",
                defaults: new { controller = "Home", action = "AllNews" }
            );

            routes.MapRoute(
                name: "get one new",
                url: "new",
                defaults: new { controller = "Home", action = "GetOneNew", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "notifications",
                url: "notifications",
                defaults: new { controller = "Home", action = "GetAllNotifications" }
            );

            routes.MapRoute(
                name: "videos",
                url: "videos",
                defaults: new { controller = "Home", action = "GetAllVideos" }
            );

            routes.MapRoute(
                name: "events",
                url: "events",
                defaults: new { controller = "Home", action = "GetAllEvents" }
            );

            routes.MapRoute(
                name: "video",
                url: "video",
                defaults: new { controller = "Home", action = "GetOneVideo" }
            );

            routes.MapRoute(
                name: "notification",
                url: "notification",
                defaults: new { controller = "Home", action = "GetOneNotification" }
            );

            routes.MapRoute(
                name: "event",
                url: "event",
                defaults: new { controller = "Home", action = "GetOneEvent" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
