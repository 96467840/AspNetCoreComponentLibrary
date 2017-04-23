using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    /*
    public class LangRoute : INamedRouter
    {
        private IRouter mvcRoute;

        public string Name { get; set; }

        public LangRoute(IServiceProvider services, string name)
        {
            mvcRoute = services.GetRequiredService<MvcRouteHandler>();
            Name = name;
        }

        public async Task RouteAsync(RouteContext context)
        {
            // парсим поддомены и переопределяем маршрут
            string requestedUrl = context.HttpContext.Request.Path.Value.TrimEnd('/');
            if (requestedUrl == "/supper")
            {
                context.RouteData.Values["controller"] = "Admin";
                context.RouteData.Values["action"] = "Index";
                context.RouteData.Values["lang"] = requestedUrl;
                await mvcRoute.RouteAsync(context);
            }
            //return Task.CompletedTask;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            //if (context.Values.ContainsKey("legacyUrl"))
            //{
            //    string url = context.Values["legacyUrl"] as string;
            //    if (!string.IsNullOrWhiteSpace(url))
            //    {
            //        return new VirtualPathData(this, "supper");
            //    }
            //}
            return null;
        }

    }/**/
}
