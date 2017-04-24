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

        // имхо так вроде как около дела. Но так валится сохранение сайта если идет в одном запросе с этим запросом
        public override void BeforeAddToCache(Users item)
        {
            if (item != null)
            {
                var usRep = Storage.GetRepository<IUserSiteRepository>(EnumDB.UserSites);
                item.UserSites = usRep.StartQuery().Where(i => i.UserId == item.Id).ToList();
            }
        }/**/

        /*
         *  совсем херово в запросе получилось
            SELECT "u"."UserId", "u"."SiteId", "u"."IsAdmin", "u"."Rights"
            FROM "UserSites" AS "u"
            INNER JOIN (
                SELECT DISTINCT "i"."Id"
                FROM "Users" AS "i"
                WHERE "i"."Id" = @__index_0
                ORDER BY "i"."Id"
                LIMIT 1
            ) AS "i0" ON "u"."UserId" = "i0"."Id"
            ORDER BY "i0"."Id" 
             
        */
        /*public override Users this[long index]
        {
            get
            {
                //if (index == null) return default(Users);

                if (coll.ContainsKey(index)) return coll[index].Item;

                var item = DbSet.Include(i => i.UserSites).FirstOrDefault(i => i.Id == index);

                AddToCache(index, item);

                return item;
            }
        }/**/

        // еще хуже. join 2 таблиц (в первом варианте джойн подзапроса хоть)
        /*
            SELECT "i.UserSites"."UserId", "i.UserSites"."SiteId", "i.UserSites"."IsAdmin", "i.UserSites"."Rights"
            FROM "Users" AS "i"
            INNER JOIN "UserSites" AS "i.UserSites" ON "i"."Id" = "i.UserSites"."UserId"
            WHERE "i"."Id" = @__item_Id_Value_0 

        */
        /*public override void BeforeAddToCache(Users item)
        {
            if (item != null && item.Id) {
                item.UserSites = DbSet.Where(i => i.Id == item.Id).SelectMany(i => i.UserSites).ToList();
            }
        }/**/

    }
}
