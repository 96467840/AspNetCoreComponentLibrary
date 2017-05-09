using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class Controller2Garin: Controller
    {
        public IStorage Storage;
        public readonly ILoggerFactory LoggerFactory;
        public readonly ILogger Logger;
        public readonly IStringLocalizerFactory LocalizerFactory;
        public readonly IStringLocalizer SharedLocalizer;

        [RepositorySettings]
        public ISiteRepository Sites { get; set; }
        [RepositorySettings]
        public IUserRepository Users { get; set; }

        public Sites Site { get; set; }
        public Users SessionUser { get; set; }
        /// <summary>
        /// Запрашиваемый язык
        /// </summary>
        public string Culture { get; set; }

        //public BaseIM Input { get; set; }

        [RepositorySettings]
        public IMenuRepository Menus { get; set; }

        public Controller2Garin(IStorage storage, ILoggerFactory loggerFactory, IStringLocalizerFactory localizerFactory)
        {
            LoggerFactory = loggerFactory;
            Logger = LoggerFactory.CreateLogger(this.GetType().FullName);
            Storage = storage;

            LocalizerFactory = localizerFactory;
            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            //_localizer = factory.Create(type);
            //SharedLocalizer = LocalizerFactory.Create("SharedResource", ""); //assemblyName);

            Logger.LogTrace("Сonstructor Controller2Garin {0}", this.GetType().FullName);
        }

        /// <summary>
        /// Проверка и установка запрашиваемого языка
        /// </summary>
        /// <param name="cultureFromGet"></param>
        [NonAction]
        public virtual void SetCulture(string cultureFromGet)
        {
            Logger.LogTrace("SetCulture {0}", cultureFromGet);

            // проверить cultureFromGet!

            // для начала примитивная (наша) проверка
            if (!cultureFromGet.TestCulture()) return;

            // так как культура идет с пути, а пути в низком регистре, то надо привести в норм вид
            cultureFromGet = cultureFromGet.ToLower();
            if (cultureFromGet.Length > 2)
            {
                // мы прошли проверку TestCulture и значит елси больше 2 букв то етсь 1 дефис
                var tmp = cultureFromGet.Explode("-");
                cultureFromGet = tmp[0] + "-" + tmp[1].ToUpper();
            }

            try
            {
                var cultureInfo = new CultureInfo(cultureFromGet);
                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

                CultureInfo.CurrentCulture = cultureInfo;
                CultureInfo.CurrentUICulture = cultureInfo;

                Culture = cultureFromGet;
            }
            catch (Exception e)
            {
                Logger.LogInformation("Cann't set culture to {0}: {1}", cultureFromGet, e);
            }
        }

        [NonAction]
        protected virtual void LoadSessionUser()
        {
            Logger.LogTrace("LoadSessionUser");
            long id = 1;
            SessionUser = Users[id];
            Logger.LogInformation("user {0} have {1} relations with sites.", id, SessionUser.UserSites.Count);

            // вот так делать нельзя! так мы пойдем по БД. права будем грузить при загрузке юзера
            //var tmp = SessionUser.UserSites.Where(i=>i.SiteId == 1);
        }

        [NonAction]
        protected void ResolveCurrentSite(ActionExecutingContext context)
        {
            // найти сайт
            var host = context.HttpContext.Request.Host.Host.ToLower();
            if (host.StartsWith("www."))
            {
                host = Regex.Replace(host, "^www.", "");
                // тут мона и редирект сделать
            }

            // на установку идем тока если БД пустая
            if (!Sites.StartQuery().Any())
            {
                context.Result = new RedirectResult(Url.RouteUrl("Setup"));
                return;
            }

            Site = Sites.StartQuery().FirstOrDefault(i=>i.TestHost(host));
            if (Site == null)
            {
                //throw new HttpException(404, "Сайт не найден.");
                context.Result = NotFound("Сайт не найден.");
                return;
            }

            // проверить права доступа
            if (!Site.IsVisible) {
                //throw new HttpException(404, "Сайт не доступен.");
                context.Result = NotFound("Сайт не доступен.");
                return;
            }

            //Logger.LogDebug("Connect to Content DB {0}", Site.Id.Value);
            // для начала мы должны определить текущий сайт
            Storage.ConnectToSiteDB(Site.Id);
        }

        //[NonAction]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Sites = Storage.GetRepository<ISiteRepository>(EnumDB.UserSites);
            Users = Storage.GetRepository<IUserRepository>(EnumDB.UserSites);

            LoadSessionUser();
            if (context.Result != null) return;

            ResolveCurrentSite(context);
            if (context.Result != null) return;

            // плохой вариант, надо заставлять девелоперов называть входные модели "input"
            /*BaseIM inputModel = context.ActionArguments.ContainsKey("input") ? (BaseIM)context.ActionArguments["input"] : null;
            if (inputModel != null)
                SetCulture(inputModel.Culture);
            /**/
            if (context.RouteData.Values.ContainsKey("Culture"))
            {
                SetCulture(context.RouteData.Values["Culture"] as string);
            }

            Menus = Storage.GetRepository<IMenuRepository>(EnumDB.Content);

            //base.OnActionExecuting(context);
        }

        public IActionResult ClearCache()
        {
            var hash = new Dictionary<string, bool>();
            foreach (var t in this.GetType().GetProperties())
            {
                // чтбы 2 раза не проверять один и тот же тип (и заодно не вызывать 2 раза очистку кеша)
                var key = t.PropertyType.Name;
                if (hash.ContainsKey(key)) continue;
                hash[key] = true;

                var ii = t.PropertyType.GetInterfaces();
                // наши репозитории всегда реализуют как минимум 2 интерфейса и один из них IRepositorySetStorageContext
                if (ii.Contains(typeof(IRepositorySetStorageContext)))
                {
                    foreach (var inter in ii)
                    {
                        //var meths = inter.GetMethods();
                        var clearcache = inter.GetMethod("ClearCache");
                        if (clearcache != null)
                        {
                            clearcache.Invoke(t.GetValue(this), null);
                            break;
                        }
                    }/**/
                }
            }
            //Sites.ClearCache();
            //Users.ClearCache();
            //Menus.ClearCache();
            return Utils.ContentResult("ClearCache Ok");
        }
    }
}
