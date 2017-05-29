using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public class SiteRepositoryProto : RepositoryWithCache<long, Sites>, ISiteRepository
    //public class SiteRepositoryProto : Repository<long, Sites>, ISiteRepository
    {
        public IQueryable<Sites> GetForUser(long userid)
        {
            // так как запрашивает это обычно текущий юзер который есть в кеше, то таким образом мы полностью добудем список из кеша
            var users = Storage.GetRepository<IUserRepository>(EnumDB.UserSites);
            var user = users[userid];
            return StartQuery().Where(i => user.UserSites.Select(us => us.SiteId).Contains(i.Id));

            /*
            var usRep = Storage.GetRepository<IUserSiteRepository>(EnumDB.UserSites);

            //List<long> ids;
            //using (new BLog(LoggerMEF, "GetForUser", GetType().FullName))
            //{
            //     ids = usRep.StartQuery().Where(i => i.UserId == userid).Select(i=>i.SiteId).ToList();
            //}
            //return StartQuery().Where(i => ids.Contains(i.Id));
            
            return StartQuery().Where(
                ii => usRep.StartQuery().Where(i => i.UserId == userid).Select(i => i.SiteId).Contains(ii.Id)
            );*/
        }

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
