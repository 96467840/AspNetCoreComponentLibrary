using AspNetCoreComponentLibrary.Abstractions;
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

    public class RepositoryWithTempCache<K,T> : Repository<K, T> where K : struct where T : BaseDM<K>
    {
        protected static ConcurrentDictionary<K, CacheItem<T>> coll = new ConcurrentDictionary<K, CacheItem<T>>();

        // время жизни кеша в секундах
        protected virtual int TimeToLife { get { return 3600; } }

        public void RemoveOldRecords()
        {
            var now = DateTime.Now;
            foreach (var item in coll.Where(i=>i.Value.Time<now)) {
                RemoveFromCache(item.Key);
            }
        }

        public override void Save(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.Id.HasValue)
            {
                DbSet.Update(item);
            }
            else
            {
                DbSet.Add(item);
            }
            Storage.Save();

            //CheckColl();
            if (item.Id.HasValue) AddToCache(item.Id.Value, item);
            AfterSave(item);
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

        public override T this[K? index]
        {
            get
            {
                if (index == null) return default(T);

                if (coll.ContainsKey(index.Value)) return coll[index.Value].Item;

                var item = DbSet.FirstOrDefault(i => i.Id.ToString() == index.ToString());
                // если юзера мы не нашли то мы все равно засунем результат в кеш чтобы 2 раза не искать в БД
                //if (item == null) { }
                AddToCache(index.Value, item);

                return item;
            }
        }

        public void RemoveFromCache(K? index)
        {
            try
            {
                //var item = this[index];
                //if (item == null) return;

                CacheItem<T> tmp;
                coll.TryRemove(index.Value, out tmp);
            }
            catch { }
        }

        public void AddToCache(K index, T item)
        {
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
