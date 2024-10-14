using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication1
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Editprofil",
                url: "{action}/{id}/{datecrea}",
                defaults: new { controller = "Home", action = "Editprofil", id = UrlParameter.Optional, datecrea = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Plogon",
                url: "{action}/{id}/{datecrea}",
                defaults: new { controller = "Home", action = "Plogon", id = UrlParameter.Optional, datecrea = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "PlogonTest",
                url: "{action}/{id}",
                defaults: new { controller = "Home", action = "PlogonTest", id = UrlParameter.Optional, datecrea = UrlParameter.Optional }
            );


        }
    }
}
