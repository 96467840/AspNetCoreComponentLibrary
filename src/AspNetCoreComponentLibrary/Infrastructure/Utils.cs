using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public static class Utils
    {
        /// <summary>
        /// Determines whether the specified HTTP request is an AJAX request.
        /// </summary>
        /// 
        /// <returns>
        /// true if the specified HTTP request is an AJAX request; otherwise, false.
        /// </returns>
        /// <param name="request">The HTTP request.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> parameter is null (Nothing in Visual Basic).</exception>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            return false;
        }

        /// <summary>
        /// приводим object к Dictionary<string, object>
        /// </summary>
        // скопипизжено http://stackoverflow.com/questions/4943817/mapping-object-to-dictionary-and-vice-versa
        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );
        }

        /// <summary>
        /// возвращаем свойство объекта в виде строки, если оно есть
        /// </summary>
        public static string GetValue(this object Object, string Name)
        {
            var propertyInfo = Object.GetType().GetProperty(Name);
            if (propertyInfo != null)
            {
                var val = propertyInfo.GetValue(Object, null);
                if (val is int)
                {
                    return ((int)val).ToString();
                }
                if (val is long)
                {
                    return ((long)val).ToString();
                }
                if (val is string)
                {
                    return (string)val;
                }
            }
            return null;
        }

        public static ContentResult ContentResult(string content)
        {
            return new ContentResult { Content = content, ContentType = "text/html" };
        }

        /// <summary>
        /// получаем имя роута
        /// </summary>
        public static string GetRouteName(this RouteData routeData)
        {
            // пиздец костылище но как эт сделать привильно я хз
            string name = null;
            foreach (var r in routeData.Routers)
            {
                //if (r.GetType().ToString() == "Microsoft.AspNetCore.Routing.Route")
                if (r.GetType() == typeof(Route))
                {
                    name = (((Route)r).Name);
                    break;
                }
            }
            return name;
        }

        /// <summary>
        /// получаем результат партиала в виде строки
        /// </summary>
        public static string GetString(this IHtmlContent content)
        {
            var writer = new System.IO.StringWriter();
            content.WriteTo(writer, HtmlEncoder.Create());
            return writer.ToString();
        }
    }
}
