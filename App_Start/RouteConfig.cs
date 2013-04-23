using System.Web.Mvc;
using System.Web.Routing;

namespace MvcSTSApplication.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default", // Route name
                            "{controller}/{action}", // URL with parameters
                            new { controller = "Home", action = "Index" } // Parameter defaults
                );
        }
    }
}