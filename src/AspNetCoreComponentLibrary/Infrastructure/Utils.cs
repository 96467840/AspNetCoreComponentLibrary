using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public static class Utils
    {
        // see here http://stackoverflow.com/questions/1895761/test-for-equality-to-the-default-value
        // проверка на значение по умолчанию
        public static bool CheckDefault<K>(K item)
        {
            return EqualityComparer<K>.Default.Equals(item, default(K));
        }

        public static bool EqualsIC(this string arg1, string arg2) {
            if (arg1 == null)
                throw new ArgumentNullException(nameof(arg1));

            // однозначно
            if (arg2 == null) return false;

            return string.Equals(arg1, arg2, StringComparison.OrdinalIgnoreCase);
        }

        public static string CryptPassword(string source)
        {
            return source;
        }

        // проверяем урл возврата чтобы не было левых урлов тока наши домены и поддомены 
        public static bool CheckBackUrl(ISiteRepository sites, string backurl)
        {
            // пустая строка или нул считается неправильной (чтобы не проверять дополнительно на верхнем уровне и пойти по альтернативной ветке)
            if (string.IsNullOrWhiteSpace(backurl)) return false;

            // локальные пути без указания домена считаем разрешенными
            if (Regex.IsMatch(backurl, @"^\w+://") || Regex.IsMatch(backurl, "^//"))
            {
                var reg = new Regex(@"^(\w+:)?//([^/:]+).*$"); // извлечь доменное имя
                var host = reg.Replace(backurl, "$2");

                if (sites.StartQuery().Any(i => i.TestHost(host))) return true;

                return false;
            }
            return true;
        }

        /// <summary>
        /// Дополняем source элементами из additional с перезаписью одинаковых ключей
        /// </summary>
        public static IDictionary<string, T> Extend<T>(this IDictionary<string, T> source, IDictionary<string, T> additional)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var kvp in additional)
            {
                source[kvp.Key] = kvp.Value;
            }

            return source;
        }

        /// <summary>
        /// Создаем новый справочник из source с дополнением элементами из additional с перезаписью одинаковых ключей. 
        /// </summary>
        public static IDictionary<string, T> ExtendNew<T>(this IDictionary<string, T> source, IDictionary<string, T> additional)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var eoColl = new Dictionary<string, T>();
            foreach (var kvp in source)
            {
                eoColl[kvp.Key] = kvp.Value;
            }
            foreach (var kvp in additional)
            {
                eoColl[kvp.Key] = kvp.Value;
            }

            return eoColl;
        }

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
        public static Dictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
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
            return routeData.DataTokens["Name"] as string;
            /*
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
            return name;/**/
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

        /// <summary>
        /// Пропускаем только по шаблону [a-z][a-z](\-[a-z][a-z])?
        /// </summary>
        public static bool TestCulture(this string culture)
        {
            if (culture == null) return false;

            var c = culture.Trim();
            var len = c.Length;
            if (len == 2) return Regex.IsMatch(c, "[a-z][a-z]", RegexOptions.IgnoreCase);

            if (len == 5 && c.IndexOf("-") == 2)
            {
                return Regex.IsMatch(c, @"[a-z][a-z]\-[a-z][a-z]", RegexOptions.IgnoreCase);
            }

            return false;
        }

        public static string[] Explode(this string s, string separator)
        {
            if (s == null) return new string[] { };
            return s.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string SendGet(this string url, Encoding encoding = null, string login = null, string pass = null, int? timeout = 60)
        {
            using (var client = login != null && pass != null ? new HttpClient(new HttpClientHandler { Credentials = new NetworkCredential(login, pass) }) : new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(timeout ?? 60);
                client.DefaultRequestHeaders.UserAgent.Clear();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36");
                if (encoding == null || encoding == Encoding.UTF8)
                {
                    return client.GetStringAsync(url).GetAwaiter().GetResult();
                }
                using (var os = client.GetStreamAsync(url).GetAwaiter().GetResult())
                using (var or = new StreamReader(os, encoding ?? Encoding.UTF8))
                {
                    return or.ReadToEnd();
                }
            }
        }

        public static string SendPost(this string url, HttpContent data, Encoding encoding = null, string login = null, string pass = null, int? timeout = 60)
        {
            using (var client = login != null && pass != null ? new HttpClient(new HttpClientHandler { Credentials = new NetworkCredential(login, pass) }) : new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(timeout ?? 60);
                client.DefaultRequestHeaders.UserAgent.Clear();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36");

                var res = client.PostAsync(url, data).GetAwaiter().GetResult();
                if (encoding == null || encoding == Encoding.UTF8)
                {
                    return res.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
                using (var os = res.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                using (var or = new StreamReader(os, encoding ?? Encoding.UTF8))
                {
                    return or.ReadToEnd();
                }
            }
        }

        public static string SendPost(this string url, IDictionary<string, string> post, Encoding encoding = null, string login = null, string pass = null, int? timeout = 60)
        {
            return SendPost(url, new FormUrlEncodedContent(post), encoding, login, pass, timeout);
        }
    }
}
