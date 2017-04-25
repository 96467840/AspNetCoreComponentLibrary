using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.EntityFrameworkCore;
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

    public class RepositoryWithTempCache<K, T> : Repository<K, T> /*where K : struct*/ where T : BaseDM<K>
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

        public override void Save(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (!BeforeSave(item)) return;
            var isnew = !Utils.CheckDefault<K>(item.Id);// !item.Id.HasValue;

            if (!isnew)
            {
                DbSet.Update(item);
            }
            else
            {
                DbSet.Add(item);
            }
            Storage.Save();

            //CheckColl();
            AddToCache(item.Id, item);
            AfterSave(item, isnew);
        }

        protected override void SetBlock(K id, bool value)
        {
            if (typeof(T) is IBlockable)
            {
                T item = (T)Activator.CreateInstance(typeof(T));
                item.Id = id;
                ((IBlockable)item).IsBlocked = value;
                DbSet.Update(item);
                Storage.Save();

                AddToCache(id, item);
            }
        }

        public override T this[K index]
        {
            get
            {
                if (index == null) return default(T);

                if (coll.ContainsKey(index)) return coll[index].Item;

                //var item = DbSet.FirstOrDefault(i => i.Id.ToString() == index.ToString());
                var item = DbSet
                    // очень важный момент!
                    .AsNoTracking()
                    .FirstOrDefault(i => i.Id.Equals(index));
                // если юзера мы не нашли то мы все равно засунем результат в кеш чтобы 2 раза не искать в БД
                //if (item == null) { }
                AddToCache(index, item);

                return item;
            }
        }

        public void RemoveFromCache(K index)
        {
            try
            {
                //var item = this[index];
                //if (item == null) return;

                coll.TryRemove(index, out CacheItem<T> tmp);
            }
            catch { }
        }

        public virtual void BeforeAddToCache(T item)
        {
        }

        public void AddToCache(K index, T item)
        {
            BeforeAddToCache(item);
            //if (Coll.ContainsKey(index)) Coll[index] = item;
            //else Coll.Add(index, item);

            // доки https://docs.microsoft.com/ru-ru/dotnet/articles/standard/collections/threadsafe/how-to-add-and-remove-items
            coll.AddOrUpdate(index, new CacheItem<T> { Time = DateTime.Now.AddSeconds(TimeToLife), Item = item }, (key, existingVal) =>
            {
                // If this delegate is invoked, then the key already exists.
                // Here we make sure the city really is the same city we already have.
                // (Support for multiple cities of the same name is left as an exercise for the reader.)
                existingVal.Time = DateTime.Now.AddSeconds(TimeToLife);
                existingVal.Item = item; // а вдруг изменился

                return existingVal;
            });
        }
    }
}
