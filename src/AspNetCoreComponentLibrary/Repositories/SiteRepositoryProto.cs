using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public class SiteRepositoryProto : RepositoryWithCache<long, Sites>//, ISiteRepository
    //public class SiteRepositoryProto : Repository<long, Sites>//, ISiteRepository
    {
        // делаем запрос по БД чтобы вытащить список юзеров для конкретного сайта
        // эта инфа нам будет нужна тока для одной страницы и запрашивать ее будем довольно редко
        // для конкретного юзера мы сделаем сохранение прав в кеш, так как юзеры у нас будут во временном кеше
        public List<UserSites> GetUserRights(long siteid)
        {
            var usRep = Storage.GetRepository<IUserSiteRepository>(EnumDB.UserSites);
            using (new BLog(LoggerMEF, "GetUserRights", GetType().FullName))
            {
                return usRep.StartQuery().Where(i => i.SiteId == siteid).ToList();
            }

        }/**/
    }
}
