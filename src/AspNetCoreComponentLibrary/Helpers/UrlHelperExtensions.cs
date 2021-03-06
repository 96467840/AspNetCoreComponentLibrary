﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

            // надо сделать автоподстановку .html
            if (routeName.EqualsIC("Page.Culture") || routeName.EqualsIC("Page"))
            {
                if (routeValues.ContainsKey("path")) // етсь путь значит к нему
                {
                    var p = (string)routeValues["path"];
                    if (p != null)
                    {
                        if (!p.EndsWith(".html"))
                        {
                            if (p.EndsWith("/"))
                                p = p.TrimEnd(new[] { '/' });
                            routeValues["path"] = p + ".html";
                        }
                    }
                }
                else // к самой странице
                {
                    if (routeValues.ContainsKey("page") && !((string)routeValues["page"]).EndsWith(".html"))
                    {
                        routeValues["page"] = (string)routeValues["page"] + ".html";
                    }
                }
            }

            // костыль для вставки пути. без него получаем 
            // @Url.RouteUrl("Page", new { page = "mypage", path = "my/long/path", getvar=1}) = /mypage/my%2flong%2fpath?getvar=1
            string path = null;
            if (routeValues.ContainsKey("path"))
            {
                path = routeValues["path"] as string;
                if (!string.IsNullOrWhiteSpace(path))
                    routeValues["path"] = "[___path___]";
            }

            // собс-но сама генерация урла
            var res = url.RouteUrl(routeName, routeValues);

            // продолжение костыля
            if (!string.IsNullOrWhiteSpace(path))
            {
                res = res.Replace("%5b___path___%5d", path);
            }

            return res;
        }

        public static string RouteUrlWithCulture(this IUrlHelper url, string routeName, object routeValues)
        {
            if (routeValues != null) return url.RouteUrlWithCulture(routeName, routeValues.AsDictionary());
            Dictionary<string, object> rv = null;
            return url.RouteUrlWithCulture(routeName, rv);
        }

        public static string CurrentUrl(this IUrlHelper Url, object replaces = null)
        {
            var rd = Url.ActionContext.RouteData;
            var routeName = rd?.GetRouteName();
            var currentRV = rd?.Values?.ToDictionary(i => i.Key, i => i.Value);
            if (currentRV == null) currentRV = new Dictionary<string, object>();
            
            // костыль для вставки пути
            /*string path = null;
            foreach (var r in currentRV)
            {
                if (r.Key == "path")
                {
                    currentRV["path"] = "[___path___]";
                    path = r.Value as string;
                    break;
                }
            }*/
            // конец костыля

            foreach (var q in Url.ActionContext.HttpContext.Request.Query)
            {
                currentRV[q.Key] = q.Value;
            }
            if (replaces != null)
            {
                foreach (var r in replaces.AsDictionary())
                {
                    currentRV[r.Key] = r.Value;
                }
            }

            var res = Url.RouteUrlWithCulture(routeName, currentRV);
            
            // костыль для вставки пути
            // решение ппц какое стремное, но пока не найду красивого и универсального решения будет так (по другому нельзя сделать path="faq/index")
            /*if (path != null)
            {
                res = res.Replace("%5b___path___%5d", path);
            }*/
            // конец костыля

            return res;
        }

    }
}
