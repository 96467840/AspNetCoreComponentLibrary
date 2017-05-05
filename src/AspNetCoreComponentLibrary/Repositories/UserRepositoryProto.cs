using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    // реализуем частичное кеширование. в кеше храним тока последних юзеров (за 1 день)
    public class UserRepositoryProto : RepositoryWithTempCache<long, Users>//, IUserRepository
    {
        protected override int TimeToLife { get { return 6400; } }

        // чтобы избежать краха следим при загрузке в кеш чтобы всегда было .AsNoTracking()
        protected override void BeforeAddToCache(Users item)
        {
            if (item != null)
            {
                var usRep = Storage.GetRepository<IUserSiteRepository>(EnumDB.UserSites);

                using (new BLog(LoggerMEF, "BeforeAddToCache", GetType().FullName))
                {
                    item.UserSites = usRep.StartQuery().Where(i => i.UserId == item.Id).ToList();
                }
            }
        }/**/

    }
}
