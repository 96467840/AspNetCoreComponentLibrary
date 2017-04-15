using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public class RouteConfig
    {
        //public string Name { get; set; }
        public string Pattern { get; set; }
        public object Defaults { get; set; }
        public RouteConfig(string pattern, object defaults)
        {
            Pattern = pattern;
            Defaults = defaults;
        }
    }

    public class DefaultRoutes
    {
        /// <summary>
        /// Регистрация стандартных правил маршрутизации с возможностью переопределения правил
        /// </summary>
        public static void Register(IRouteBuilder routes, IApplicationBuilder app, Dictionary<string, RouteConfig> replace = null)
        {
            var defaults = new Dictionary<string, RouteConfig> {
                { "Setup", new RouteConfig("setup", new { controller = "Setup", action = "Index" })},
                { "Page", new RouteConfig("{lang?}/{page?}/", new { controller = "Home", action = "Index" })},
            };

            if (replace != null)
            {
                defaults.Extend(replace);
            }

            foreach (var route in defaults)
            {
                routes.MapRoute(route.Key, route.Value.Pattern, route.Value.Defaults);
            }

            //routes.MapRoute("Setup", "setup", new { controller = "Setup", action = "Index" });
            //routes.MapRoute("Page", "{lang?}/{page?}/", new { controller = "Home", action = "Index" });

            //routes.MapRoute(name: "default",  template: "{controller=Home}/{action=Index}/{id?}");
        }

    }
}
