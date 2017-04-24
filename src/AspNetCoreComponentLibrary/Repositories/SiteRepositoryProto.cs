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
        //
        // очень плохое решение см UserRepositoryProto (там делал сранения вариантов)
        /*public List<UserSites> GetUserRights(long siteid)
        {
            return DbSet.Where(i => i.Id == siteid).SelectMany(i => i.UserSites).ToList();
        }/**/
    }
}
