using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public static class UrlHelperExtensions
    {
        // для таких правил вызываем обычный RouteUrl
        // имена роутов без поддержки культуры
        //public static List<string> Excludes = new List<string>() { "Setup" };

        public static string RouteUrlWithCulture(this IUrlHelper url, string routeName, Dictionary<string, object> routeValues)
        {
            //if (Excludes.Contains(routeName)) return url.RouteUrl(routeName, routeValues);

            // перезапишем культуру из текущего контекста (если культура не указана явно в новом наборе)
            var currentRV = url.ActionContext.RouteData.Values;
            if (currentRV.ContainsKey("culture"))
            {
                if (routeValues == null) routeValues = new Dictionary<string, object>();
                if (!routeValues.ContainsKey("culture")) routeValues["culture"] = currentRV["culture"];
            }

            if (routeValues != null && routeValues.ContainsKey("culture") && routeName.IndexOf(".") < 0)
            {
                routeName += ".Culture";
            }
            
            return url.RouteUrl(routeName, routeValues);
        }

        public static string RouteUrlWithCulture(this IUrlHelper url, string routeName, object routeValues)
        {
            if (routeValues != null) return url.RouteUrlWithCulture(routeName, routeValues.AsDictionary());
            return url.RouteUrlWithCulture(routeName, (Dictionary<string, object>)null);
        }
    }
}
