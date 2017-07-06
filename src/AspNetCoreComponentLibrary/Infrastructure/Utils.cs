using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.WebEncoders;
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
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerPrefix"></param>
        /// <param name="attributePrefix"></param>
        /// <param name="propertyName"></param>
        /// <param name="type"></param>
        /// <param name="anyProperty">например, common.field.any.regexpmessage</param>
        /// <returns></returns>
        public static List<string> GenLocalizeKeysList(string controllerPrefix, string attributePrefix, string propertyName, string type, bool anyProperty = false)
        {
            var keys = new List<string>();
            if (!string.IsNullOrWhiteSpace(attributePrefix) && attributePrefix != "field")
            {
                keys.Add(controllerPrefix + "." + attributePrefix + "." + propertyName + "." + type);
                keys.Add("common." + attributePrefix + "." + propertyName + "." + type);
                if (anyProperty)
                {
                    keys.Add(controllerPrefix + "." + attributePrefix + ".any." + type);
                    keys.Add("common." + attributePrefix + ".any." + type);
                }
            }
            keys.Add(controllerPrefix + ".field." + propertyName + "." + type);
            keys.Add("common.field." + propertyName + "." + type);
            if (anyProperty)
            {
                keys.Add(controllerPrefix + ".field.any." + type);
                keys.Add("common.field.any." + type);
            }
            return keys;
        }

        public static Form GetForm<FA>(this Type t, Controller2Garin controller) where FA : FieldBaseAttribute
        {
            var fields = new List<IField>();

            foreach (var f in t.GetProperties()
                .Select(i => new { Attribute = (FA)i.GetCustomAttribute(typeof(FA)), Property = i, }).Where(i => i.Attribute != null))
            {
                var title = controller.Localizer2Garin.Localize(GenLocalizeKeysList(controller.LocalizerPrefix, f.Attribute.LocalizePrefix , f.Property.Name, "title"));

                var placeholder = controller.Localizer2Garin.Localize(GenLocalizeKeysList(controller.LocalizerPrefix, f.Attribute.LocalizePrefix, f.Property.Name, "placeholder"));

                IField field = null;
                Type[] typeArgs = { f.Property.PropertyType };
                Type d1, ff;
                switch (f.Attribute.HtmlType)
                {
                    case EnumHtmlType.Image:
                        break;
                    case EnumHtmlType.Images:
                        break;
                    case EnumHtmlType.File:
                        break;
                    case EnumHtmlType.Files:
                        break;
                    case EnumHtmlType.Select:
                        d1 = typeof(FieldSelect<>);
                        ff = d1.MakeGenericType(typeArgs);
                        field = (IField)Activator.CreateInstance(ff, new object[] { controller, title, placeholder, f.Attribute.HtmlType, f.Property.Name, f.Attribute.Priority, f.Attribute.Default });
                        break;
                    default:
                        d1 = typeof(Field<>);
                        ff = d1.MakeGenericType(typeArgs);
                        field = (IField)Activator.CreateInstance(ff, new object[] { controller, title, placeholder, f.Attribute.HtmlType, f.Attribute.NeedTranslate, f.Property.Name, f.Attribute.Priority, f.Attribute.Default });
                        break;
                }
                if (field != null)
                {
                    field.Priority = f.Attribute.Priority;

                    fields.Add(field);
                }
            }

            return new Form(controller, fields);
        }

        /// <summary>
        /// Получаем значение параметра с запроса. Также будет попытка получить значение с RouteData
        /// </summary>
        /// <param name="request"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<string> GetRequestValue(this HttpRequest request, string name)
        {
            // get данные могут быть и в посте! надо определить очередность
            if (request.Query.ContainsKey(name)) return request.Query[name].ToList();

            // post
            if (request.Method.EqualsIC("post") && request.Form.ContainsKey(name)) return request.Form[name].ToList();

            // возможно нужные нам данные есть в пути, то есть в RouteData. Причем в RouteData всегда одиночное значение
            var rd = request.HttpContext.GetRouteData();
            if (rd.Values.ContainsKey(name)) return new List<string>() { rd.Values[name].ToString() };
            return null;
        }

        public static bool EqualsIC(this string arg1, string arg2) {
            if (arg1 == null)
                throw new ArgumentNullException(nameof(arg1));

            // однозначно
            if (arg2 == null) return false;

            return string.Equals(arg1, arg2, StringComparison.OrdinalIgnoreCase);
        }

        public static void Set2GarinServices<L>(this IServiceCollection services, IConfigurationRoot Configuration) where L : class, IStringLocalizer
        {
            // необходимо для NLog (без этого в логах не будет текущего запроса)
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            // Setup options with DI
            services.AddOptions();

            services.Configure<LocalizerConfigure>(Configuration.GetSection("LocalizerConfigure")/*, trackConfigChanges: true*/);

            // Add framework services.
            services.AddMvc()
                // локализацию шаблонов мы не юзаем, но мы юзаем инжект на вьюхах (IViewLocalizer)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix) //Adds support for localized view files. In this sample view localization is based on the view file suffix. For example "fr" in the Index.fr.cshtml file.
                .AddDataAnnotationsLocalization();

            // чтобы во вьюхах русские символы не кодировались
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });/**/

            // зачем его инжектить если его всегда можно создать и сохранить в статике?
            // https://docs.microsoft.com/en-us/aspnet/core/security/cross-site-scripting
            //services.AddSingleton<HtmlEncoder>(
            //    HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));

            // генерируем урлы в низком регистре (так как для и линукса делаем, а там привычнее когда все пути в низком регистре)
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            // конфигурируем подгрузку представлений из библиотеки
            var assembly = typeof(AspNetCoreComponentLibrary.TestComponent).GetTypeInfo().Assembly;
            var embededFileProvider = new EmbeddedFileProvider(assembly, "AspNetCoreComponentLibrary");
            services.Configure<RazorViewEngineOptions>(options => { options.FileProviders.Add(embededFileProvider); });

            // https://docs.microsoft.com/ru-ru/aspnet/core/performance/caching/middleware
            //services.AddResponseCaching();

            services.AddSingleton<IStringLocalizer, L>();

            services.AddScoped<ILocalizer2Garin, Localizer2Garin>();

            services.AddTransient<IControllerSettings, ControllerSettings>();
        }

        /*public static HtmlEncoder _enc;
        public static HtmlEncoder GetHtmlEncoder() {
            if (_enc == null)
                _enc = HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All });
            return _enc;
        }*/
        public static string StripHtml(this string source)
        {
            if (source == null) return null;

            return HtmlUtility.StripHtml(source);// GetHtmlEncoder().Encode(source);
        }

        /// <summary>
        /// Разрешаем только безопасные html теги.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SanitizeHtml(this string source)
        {
            if (source == null) return null;

            return HtmlUtility.SanitizeHtml(source);
        }

        public static string CryptPassword(string source)
        {
            return source;
        }

        public static bool IsImplementsInterface(this Type T, Type I)
        {
            return I.GetTypeInfo().IsAssignableFrom(T);
        }

        public static string ToJson(this object obj)
        {
            var str = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                //ContractResolver = new CamelCasePropertyNamesContractResolver(),
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
                var str = string.Join("\n", l.GetAllStrings().Select(i => i.Name + "->" + i.Value).Take(2).ToList());
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

                if (sites.StartQuery(0).Any(i => i.TestHost(host))) return true;

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
