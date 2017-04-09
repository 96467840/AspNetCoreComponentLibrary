using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Mvc;
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

        protected void ResolveCurrentSite()
        {
            // найти сайт
            var host = this.Request.Host.Host.ToLower();
            if (host.StartsWith("www."))
            {
                host = Regex.Replace(host, "^www.", "");
                // тут мона и редирект сделать
            }
            Site = Sites.StartQuery().FirstOrDefault(i=>i.Hosts == host);
            if (Site == null)
            {
                // поиск по альтернативному имени
                // попозжа ...
            }

            if (Site == null)
            {
                throw new HttpException(404, "Сайт не найден.");
            }

            // проверить права доступа
            if (!Site.IsVisible) throw new HttpException(404, "Сайт не доступен.");

            // для начала мы должны определить текущий сайт
            Storage.ConnectToSiteDB(Site.Id.Value);
        }

        public Controller2Garin(IStorage storage, ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            Logger = LoggerFactory.CreateLogger(this.GetType().FullName);
            Storage = storage;

            Sites = Storage.GetRepository<ISiteRepository>(false);
            Users = Storage.GetRepository<IUserRepository>(false);

            ResolveCurrentSite();

            Menus = Storage.GetRepository<IMenuRepository>(true);

        }

    }
}
