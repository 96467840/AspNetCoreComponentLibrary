using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public IMenuRepository Menus { get; set; }


        public Controller2Garin(IStorage storage, ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            Logger = LoggerFactory.CreateLogger(this.GetType().FullName);
            Storage = storage;
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

            // для начала мы должны определить текущий сайт
            Storage.ConnectToSiteDB(Site.Id.Value);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Sites = Storage.GetRepository<ISiteRepository>(false);
            Users = Storage.GetRepository<IUserRepository>(false);

            ResolveCurrentSite(context);
            if (context.Result != null) return;

            Menus = Storage.GetRepository<IMenuRepository>(true);
        }

    }
}
