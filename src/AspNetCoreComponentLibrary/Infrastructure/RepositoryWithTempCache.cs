using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public class CacheItem<T> {
        public DateTime Time { get; set; }
        public T Item { get; set; }
    }

    public class RepositoryWithTempCache<K, T> : Repository<K, T> where T : BaseDM<K>
    {
        protected static ConcurrentDictionary<K, CacheItem<T>> coll = new ConcurrentDictionary<K, CacheItem<T>>();

        // время жизни кеша в секундах
        protected virtual int TimeToLife { get { return 3600; } }

        public void RemoveOldRecords()
        {
            var now = DateTime.Now;
            foreach (var item in coll.Where(i => i.Value.Time < now)) {
                RemoveFromCache(item.Key);
            }
        }

        public override void Remove(T item)
        {
            var id = item.Id;
            DbSet.Remove(item);
            (StorageContext as DbContext).SaveChanges();
            RemoveFromCache(id);
        }

        public override void AfterSave(T item, bool isnew)
        {
            base.AfterSave(item, isnew);
            Logger.LogTrace("RepositoryWithCache AfterSave for {0}", item.Id);
            AddToCache(item.Id);
        }/**/

        public override T Save(T item)
        {
            if (item == null) throw new ArgumentNullException();

            var clone = item.CloneJson();

            if (!BeforeSave(item)) return item;
            var isnew = Utils.CheckDefault(clone.Id);

            if (!isnew)
            {
                DbSet.Update(clone);
            }
            else
            {
                DbSet.Add(clone);
            }
            (StorageContext as DbContext).SaveChanges();
            AfterSave(clone, isnew);
            return clone;
        }

        protected override void SetBlock(K id, bool value)
        {
            //if (typeof(T) is IBlockable)
            if (typeof(T).IsImplementsInterface(typeof(IBlockable)))
            {
                T item = (T)Activator.CreateInstance(typeof(T));
                item.Id = id;
                ((IBlockable)item).IsBlocked = value;
                //DbSet.Update(item);
                Save(item);
            }
        }

        public override T this[K index]
        {
            get
            {
                if (index == null) return default(T);

                if (coll.ContainsKey(index)) return coll[index].Item;

                // добавление в кеш вытащит сущность заново
                AddToCache(index);

                if (coll.ContainsKey(index)) return coll[index].Item;

                return null;
            }
        }

        public override void RemoveFromCache(K index)
        {
            try
            {
                coll.TryRemove(index, out CacheItem<T> tmp);
            }
            catch { }
        }

        protected virtual void BeforeAddToCache(T item)
        {
        }/**/

        public override void AddToCache(K index)
        {
            // item ни в коем случае нельзя помещать в кеш (трэкинг включен после сохранения)
            var newitem = GetSingleFromDB(index);

            BeforeAddToCache(newitem);

            // доки https://docs.microsoft.com/ru-ru/dotnet/articles/standard/collections/threadsafe/how-to-add-and-remove-items
            coll.AddOrUpdate(index, new CacheItem<T> { Time = DateTime.Now.AddSeconds(TimeToLife), Item = newitem }, (key, existingVal) =>
            {
                // If this delegate is invoked, then the key already exists.
                // Here we make sure the city really is the same city we already have.
                // (Support for multiple cities of the same name is left as an exercise for the reader.)
                existingVal.Time = DateTime.Now.AddSeconds(TimeToLife);
                existingVal.Item = newitem; // а вдруг изменился

                return existingVal;
            });
        }

        public override void ClearCache(long? siteid)
        {
            Logger.LogTrace("RepositoryWithTempCache ClearCache {0} for siteid={1}", this.GetType().FullName, siteid);
            coll.Clear();
        }

    }
}
