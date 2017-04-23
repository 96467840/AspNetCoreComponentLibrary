using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreComponentLibrary
{
    /// <summary>
    /// Пропускаем только по шаблону \w\w(-\w\w)?
    /// </summary>
    public class CultureRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var culture = (values[routeKey] ?? "").ToString();
            return culture.TestCulture();
        }
    }

    public class RouteConfig
    {
        //public string Name { get; set; }
        public string Pattern { get; set; }
        public object Defaults { get; set; }
        public object Constraints { get; set; }
        public object DataTokens { get; set; }

        public int Priority { get; set; }
        public RouteConfig(int priority, string pattern, object defaults, object constraints = null, object dataTokens =null)
        {
            Priority = priority;
            Pattern = pattern;
            Defaults = defaults;
            Constraints = constraints;
            DataTokens = dataTokens;
        }
    }

    public class DefaultRoutes
    {
        /// <summary>
        /// Регистрация стандартных правил маршрутизации с возможностью переопределения правил. Удалить правило нельзя, но его можно убрать в самый конец (priority > 100 000)
        /// Свои админские контролеры располагать до priority = 10 000 (на этом уровне определен универсальный админский маршрут)
        /// </summary>
        public static void Register(IRouteBuilder routes, IApplicationBuilder app, Dictionary<string, RouteConfig> replace = null)
        {
            var defaults = new Dictionary<string, RouteConfig> {
                { "Setup",         new RouteConfig(0,      "setup", new { controller = "Setup", action = "Index" })},

                { "Admin.Culture", new RouteConfig(10000,  "{culture}/admin/{controller}/{action}/", new { action = "List" }, new { culture = new CultureRouteConstraint() })},
                { "Admin" ,        new RouteConfig(10001,  "admin/{controller}/{action}/", new { action = "List" })},

                { "Page.Culture",  new RouteConfig(100000, "{culture}/{page?}/{*path}", new { controller = "Home", action = "Index", page = "index.html" }, new { culture = new CultureRouteConstraint() })},
                { "Page",          new RouteConfig(100001, "{page?}/{*path}", new { controller = "Home", action = "Index", page = "index.html" })},
            };

            if (replace != null)
            {
                defaults.Extend(replace);
            }

            foreach (var route in defaults.OrderBy(i=>i.Value.Priority))
            {
                var dataTokens = route.Value.DataTokens;
                var name = route.Key;

                // здесь именно > 0 ! если точка на первом месте => имя будет пустым что плохо
                if (name.IndexOf(".") > 0) name = name.Explode(".")[0];

                if (dataTokens == null) dataTokens = new { Name = name };
                routes.MapRoute(route.Key, route.Value.Pattern, route.Value.Defaults, route.Value.Constraints, dataTokens);
            }

            //routes.MapRoute("Setup", "setup", new { controller = "Setup", action = "Index" });
            //routes.MapRoute("Page", "{lang?}/{page?}/", new { controller = "Home", action = "Index" });

            //routes.MapRoute(name: "default",  template: "{controller=Home}/{action=Index}/{id?}");
        }

    }
}
