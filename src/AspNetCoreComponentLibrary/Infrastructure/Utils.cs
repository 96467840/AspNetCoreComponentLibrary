using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public static string MEFNameSpace = "Microsoft.EntityFrameworkCore.Our";

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

        public static bool IsImplementsInterface(this Type T, Type I)
        {
            return I.GetTypeInfo().IsAssignableFrom(T);
        }

        /// <summary>
        /// Очищаем строку от опасных html инструкций. Пока не реализовано! Пока удаляем все html теги
        /// </summary>
        public static string SanitizeHtml(this string source, bool removeAllHtml = true)
        {
            if (source == null) return null;
            if (removeAllHtml) return source.Replace("<", "&lt;").Replace(">","&gt;");

            return source.Replace("<", "");
        }

        public static string ToJson(this object obj)
        {
            var str = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                StringEscapeHandling = StringEscapeHandling.Default
            });
            //return str.Replace("\\u003cbr /\\u003e", "<br />");
            return str;
        }

        public static IStringLocalizer LoadCulture(this IStringLocalizer localizer, string culture, ILogger logger)
        {
            try
            {
                var cultureInfo = new CultureInfo(culture);
                var l = localizer.WithCulture(cultureInfo);
                // проверим культуру
                // если такой кульутры нет, то в этом месте будет эксепшен (при вызове GetAllStrings). 
                // причем как я понял это единственный способ узнать есть ли такая культура в ресурсах или нет
                var str = string.Join("\n", l.GetAllStrings().Select(i => i.Name + "->" + i.Value).ToList());
                if (logger != null)
                {
                    logger.LogTrace("LoadCulture for {0} strings:\n{1}", localizer.GetType().FullName, str);
                }
                return l;
            }
            catch (Exception e)
            {
                logger.LogTrace("LoadCulture for {0} fail. {1}", localizer.GetType().FullName, e);
                return null;
            }
        }

        // скопировано отсюда http://stackoverflow.com/questions/78536/deep-cloning-objects
        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method. NOTE: Private members are not cloned using this method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CloneJson<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
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
        public static IDictionary<K, T> Extend<K, T>(this IDictionary<K, T> source, IDictionary<K, T> additional)
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
        public static IDictionary<K, T> ExtendNew<K, T>(this IDictionary<K, T> source, IDictionary<K, T> additional)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var eoColl = new Dictionary<K, T>();
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
        public static string GetObjectValue(this object Object, string Name)
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
        /// Пропускаем только по шаблону [a-z][a-z](\-[A-Z][A-Z])?
        /// </summary>
        public static bool TestCulture(this string culture)
        {
            if (culture == null) return false;

            var c = culture.Trim();
            var len = c.Length;
            if (len == 2) return Regex.IsMatch(c, "[a-z][a-z]", RegexOptions.IgnoreCase);

            if (len == 5 && c.IndexOf("-") == 2)
            {
                return Regex.IsMatch(c, @"[a-z][a-z]\-[A-Z][A-Z]", RegexOptions.IgnoreCase);
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
