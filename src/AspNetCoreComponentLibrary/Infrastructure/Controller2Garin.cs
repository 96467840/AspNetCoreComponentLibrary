using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class Controller2Garin: Controller
    {
        public IStorage Storage;
        public readonly ILoggerFactory LoggerFactory;
        public readonly ILogger Logger;

        public ISiteRepository Sites { get; set; }
        public IUserRepository Users { get; set; }

        public Sites Site { get; set; }
        public Users SessionUser { get; set; }

        public IMenuRepository Menus { get; set; }

        public Controller2Garin(IStorage storage, ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            Logger = LoggerFactory.CreateLogger(this.GetType().FullName);
            Storage = storage;
        }

        protected void LoadSessionUser()
        {
            Logger.LogTrace("LoadSessionUser");
            long id = 1;
            SessionUser = Users[id];
            Logger.LogInformation("user {0} have {1} relations with sites.", id, SessionUser.UserSites.Count);

            // вот так делать нельзя! так мы пойдем по БД. права будем грузить при загрузке юзера
            //var tmp = SessionUser.UserSites.Where(i=>i.SiteId == 1);
        }

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

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Sites = Storage.GetRepository<ISiteRepository>(EnumDB.UserSites);
            Users = Storage.GetRepository<IUserRepository>(EnumDB.UserSites);

            LoadSessionUser();
            if (context.Result != null) return;

            ResolveCurrentSite(context);
            if (context.Result != null) return;

            Menus = Storage.GetRepository<IMenuRepository>(EnumDB.Content);
        }

        public IActionResult ClearCache()
        {
            foreach (var t in this.GetType().GetProperties())
            {
                var ii = t.PropertyType.GetInterfaces();
                // наши репозитории всегда реализуют как минимум 2 интерфейса и один из них IRepositorySetStorageContext
                if (ii.Contains(typeof(IRepositorySetStorageContext)))
                {
                    foreach (var inter in ii)
                    {
                        var meths = inter.GetMethods();
                        var clearcache = inter.GetMethod("ClearCache");
                        if (clearcache != null)
                        {
                            clearcache.Invoke(t.GetValue(this), new object[] { });
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
