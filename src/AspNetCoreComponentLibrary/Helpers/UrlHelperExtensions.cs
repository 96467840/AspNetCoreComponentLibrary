using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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

            if (routeValues != null && routeValues.ContainsKey("culture"))
            {
                if (string.IsNullOrWhiteSpace(routeValues["culture"] as string))
                {
                    // культура принудительно пуста, ничего не делаем так пустую и передаем, тока чистим имя роута
                    if (routeName.IndexOf(".") >= 0)
                    {
                        routeName = Regex.Replace(routeName, @"\.Culture$", "");
                    }
                    routeValues["culture"] = null; // хотя бы для того чтобы убить пустую строку
                }
                else
                {
                    if (routeName.IndexOf(".") < 0)
                    {
                        routeName += ".Culture";
                    }
                }

            }
            
            var res = url.RouteUrl(routeName, routeValues);
            return res;
        }

        public static string RouteUrlWithCulture(this IUrlHelper url, string routeName, object routeValues)
        {
            if (routeValues != null) return url.RouteUrlWithCulture(routeName, routeValues.AsDictionary());
            Dictionary<string, object> rv = null;
            return url.RouteUrlWithCulture(routeName, rv);
        }
    }
}
