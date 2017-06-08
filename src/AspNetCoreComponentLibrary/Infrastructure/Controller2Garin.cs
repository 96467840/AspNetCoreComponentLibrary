using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Localization;
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
        public readonly ILogger LoggerMEF;
        public readonly ILocalizer2Garin Localizer2Garin;

        [RepositorySettings]
        public ISiteRepository Sites { get; set; }
        [RepositorySettings]
        public IUserRepository Users { get; set; }

        /// <summary>
        /// Префикс для локализации
        /// </summary>
        public string LocalizerPrefix { get; set; }

        /// <summary>
        /// Текущий сайт
        /// </summary>
        public Sites Site { get; set; }

        /// <summary>
        /// Текущий юзер
        /// </summary>
        public Users SessionUser { get; set; }

        /// <summary>
        /// Список языков сайта Site
        /// </summary>
        public List<Languages> SiteLanguages { get; set; }

        #region Content Repositories

        private T GetContentRepository<T>() where T:IRepositorySetStorageContext
        {
            return Storage.GetRepository<T>(EnumDB.Content);
        }

        [RepositorySettings]
        public IMenuRepository Menus => GetContentRepository<IMenuRepository>();

        [RepositorySettings]
        public ILanguageRepository Languages => GetContentRepository<ILanguageRepository>();

        #endregion

        public Controller2Garin(IControllerSettings settings)
        {
            LoggerFactory = settings.LoggerFactory;
            Logger = LoggerFactory.CreateLogger(this.GetType().FullName);
            // для красоты в логах EntityFrameworkCore
            LoggerMEF = LoggerFactory.CreateLogger(Utils.MEFNameSpace);
            Localizer2Garin = settings.Localizer2Garin;

            Logger.LogTrace("-=-= Сonstructor Controller2Garin {0}", this.GetType().FullName);

            Storage = settings.Storage;

            //LocalizerFactory = settings.LocalizerFactory;
            

            Logger.LogTrace("end of Сonstructor Controller2Garin {0}", this.GetType().FullName);
        }

        [NonAction]
        protected virtual void LoadSessionUser()
        {
            Logger.LogTrace("LoadSessionUser");
            long id = 1;
            try
            {
                SessionUser = Users[id];
            }
            catch (Exception e)
            {
                Logger.LogDebug("User {id} not founded: {e}", id, e);
            }
            //Logger.LogInformation("user {0} have {1} relations with sites.", id, SessionUser.UserSites.Count);

            // вот так делать нельзя! так мы пойдем по БД. права будем грузить при загрузке юзера
            //var tmp = SessionUser.UserSites.Where(i=>i.SiteId == 1);
        }

        [NonAction]
        protected void ResolveCurrentSite(ActionExecutingContext context)
        {
            Logger.LogTrace("============================= ResolveCurrentSite in");

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
            Logger.LogTrace("============================= OnActionExecuting in");

            Sites = Storage.GetRepository<ISiteRepository>(EnumDB.UserSites);
            Users = Storage.GetRepository<IUserRepository>(EnumDB.UserSites);

            LoadSessionUser();
            if (context.Result != null) return;

            ResolveCurrentSite(context);
            if (context.Result != null) return;

            SiteLanguages = Languages.GetUnblocked(Site.Id).ToList();
            Localizer2Garin.ResolveCulture(context, SiteLanguages, Url);
            
            if (context.Result != null) return;

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

        public HtmlString Localize(string key)
        {
            return Localizer2Garin.Localize(key);
        }

    }
}
