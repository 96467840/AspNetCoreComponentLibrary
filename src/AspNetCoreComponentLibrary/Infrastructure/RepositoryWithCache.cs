using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace AspNetCoreComponentLibrary
{
    public abstract class RepositoryWithCache<K, T> : Repository<K, T> where T : BaseDM<K>
    {
        protected static Dictionary<long, Dictionary<K, T>> coll;

        public virtual void LoadFromDB()
        {
            //Logger.LogTrace("........ LoadFromDB");
            //Thread.Sleep(1000);
            using (new BLog(LoggerMEF, "LoadFromDB", GetType().FullName))
                if (typeof(T).IsImplementsInterface(typeof(IWithSiteId)))
                {
                    coll = DbSet
                        // очень важный момент!
                        .AsNoTracking()
                        .ToList()
                        .GroupBy(i => ((IWithSiteId)i).SiteId).ToDictionary(i => i.Key, i => i.ToDictionary(j => j.Id, j => j));
                }
                else
                {
                    coll = new Dictionary<long, Dictionary<K, T>> {
                        {
                            0, DbSet
                            // очень важный момент!
                            .AsNoTracking()
                            .ToDictionary(i => i.Id, i => i)
                        }
                    };
                }
            //Logger.LogTrace("........ end of LoadFromDB");
        }

        private static Object thisLock = new Object();
        protected void CheckColl()
        {
            //Logger.LogTrace("........ CheckColl in");
            // блокировка (абсолютно нет смысла делать еще один раз запрос к БД, проще тупо дождаться результата первого)
            lock (thisLock)
            {
                //Logger.LogTrace("........ CheckColl after lock");
                if (coll == null) LoadFromDB();
                //Logger.LogTrace("........ CheckColl after load");
            }
            if (coll == null) throw new Exception(string.Format("Can't load collection {0} from DB", typeof(T).FullName));
        }

        public override IQueryable<T> StartQuery(long siteid)
        {
            CheckColl();
            if (typeof(T).IsImplementsInterface(typeof(IWithSiteId)))
            {
                if (siteid > 0)
                {
                    if (coll.ContainsKey(siteid))
                        return coll[siteid].Values.AsQueryable();
                }
                else
                {
                    // попытка построить запрос по нескольким сайтам?  в теории такое может понадобится для аналитики => такие запросы идут мимо кеша
                    return DbSet.AsNoTracking();
                }
                // такого сайта нет в кеше значит его нет и в БД => вернем пустой запрос (по идее это ошибка, но сайт мы ломать не станем)
                //Logger.LogCritical("Try build query for 'IWithSiteId' with siteid={siteid}, but site not founded in cache.", siteid);
                // и такое возможно если у сайта нет сущностей такого типа => смело шлем пустой результат
                return new List<T>().AsQueryable();
            }

            // запрос по сущностям без привязки к сайту (пока их 2 это сам сайт и юзер)
            if (coll.ContainsKey(0))
                return coll[0].Values.AsQueryable();

            // критическая ситуация в CheckColl мы проверили и инициализировали кеш и сущности без сайта должны быть в нулевом индексе
            // в этом случае вернем пустой запрос чтобы не ломать приложение
            Logger.LogCritical("Try build query for non 'IWithSiteId' with siteid={siteid}, but site not founded in cache.", siteid);
            return new List<T>().AsQueryable();
            //return DbSet.AsNoTracking();
        }

        public override T this[K index]
        {
            get
            {
                CheckColl();
                var s = coll.Values.Where(i => i.ContainsKey(index)).FirstOrDefault();
                if (s == null) return default(T);
                return s[index];
            }
        }

        // вот за что не навижу ормы. если мы попытаемся вызвать Save с объектом из кеша, 
        // и у нас произойдет ошибка (причем именно при ошибке), то он сцуко связи втянет в кеш
        // чтобы это предотвратить объект надо клонировать или как-то убрать связи (убрать связи нельзя, так как генерируется код с этими связями)
        public override void Save(T item)
        {
            if (item == null) throw new ArgumentNullException();

            var clone = item.CloneJson();
            if (!BeforeSave(clone)) return;
            var isnew = Utils.CheckDefault(clone.Id);
            if (!isnew)
            {
                DbSet.Update(clone);
            }
            else
            {
                DbSet.Add(clone);
            }
        }

        public override void AfterSave(T item, bool isnew)
        {
            base.AfterSave(item, isnew);
            Logger.LogTrace("RepositoryWithCache AfterSave for {0}", item.Id);
            AddToCache(item.Id);
        }/**/

        public override void Remove(T item)
        {
            RemoveFromCache(item.Id);
            DbSet.Remove(item);
        }

        protected override void SetBlock(K id, bool value)
        {
            if (typeof(T).IsImplementsInterface(typeof(IBlockable)))
            {
                T item = (T)Activator.CreateInstance(typeof(T));
                item.Id = id;
                ((IBlockable)item).IsBlocked = value;
                DbSet.Update(item);
            }
        }

        /// <summary>
        /// Чистим кеш. Если указан конкретный сайт, то кеш для него будет ПЕРЕЗАГРУЖЕН заново.
        /// </summary>
        /// <param name="siteid"></param>
        public override void ClearCache(long? siteid)
        {
            Logger.LogTrace("RepositoryWithCache ClearCache {0} for siteid={1}", this.GetType().FullName, siteid);
            if (siteid.HasValue)
            {
                if (coll != null)
                {
                    if (typeof(T).IsImplementsInterface(typeof(IWithSiteId)))
                    {
                        coll[siteid.Value] = DbSet
                        // очень важный момент!
                        .AsNoTracking()
                        .Where(i => ((IWithSiteId)i).SiteId == siteid.Value)
                        .ToDictionary(j => j.Id, j => j);
                    }
                    else
                    {
                        coll = null;
                    }
                }
            }
            else
            {
                coll = null;
            }
        }

        protected void RemoveFromCache(K index)
        {
            CheckColl();
            try
            {
                var s = coll.Values.Where(i => i.ContainsKey(index)).FirstOrDefault();
                if (s == null) return;
                s.Remove(index);
            }
            catch { }
        }

        protected virtual void BeforeAddToCache(T item)
        {
        }/**/

        protected void AddToCache(K index)
        {
            CheckColl();

            var newitem = GetSingleFromDB(index);

            BeforeAddToCache(newitem);

            if (typeof(T).IsImplementsInterface(typeof(IWithSiteId)))
            {
                if (!coll.ContainsKey(((IWithSiteId)newitem).SiteId)) coll[((IWithSiteId)newitem).SiteId] = new Dictionary<K, T>();
                coll[((IWithSiteId)newitem).SiteId][index] = newitem;
            }
            else
            {
                if (!coll.ContainsKey(0)) coll[0] = new Dictionary<K, T>();
                coll[0][index] = newitem;
            }
        }
    }
}
