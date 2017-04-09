using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public abstract class Repository<K, T> where K : struct where T : BaseDM<K>
    {
        protected DbSet<T> _dbSet = null;
        protected DbSet<T> dbSet
        {
            get
            {
                //(Storage as Storage)._logger.LogInformation("try get dbSet...");
                if (_dbSet != null) return _dbSet;
                _dbSet = (StorageContext as DbContext).Set<T>();
                return _dbSet;
            }
        }

        protected IStorageContext StorageContext;
        protected IStorage Storage { get; set; }

        public void SetStorageContext(IStorageContext storageContext, IStorage storage)
        {
            StorageContext = storageContext;
            Storage = storage;
        }

        public IQueryable<T> StartQuery() {
            return dbSet.AsNoTracking();
        }

        public void Save(T item)
        {
            if (item == null) throw new ArgumentNullException();
            if (item.Id.HasValue)
            {
                dbSet.Update(item);
            }
            else
            {
                dbSet.Add(item);
            }
            Storage.Save();
        }

        public void Remove(K id)
        {
            var item = this[id];
            if (item == null) return;
            dbSet.Remove(item);
            Storage.Save();
        }

        public void SetBlock(K id, bool value)
        {
            if (typeof(T) is IBlockable)
            {
                T item = (T)Activator.CreateInstance(typeof(T));
                item.Id = id;
                ((IBlockable)item).IsBlocked = value;
                dbSet.Update(item);
                Storage.Save();
            }
        }

        public void Block(K id) { SetBlock(id, true); }
        public void UnBlock(K id) { SetBlock(id, false); }

        public T this[K? index]
        {
            get
            {
                if (index == null) return default(T);
                return dbSet.FirstOrDefault(i => i.Id.ToString() == index.ToString());
                //return dbSet.FirstOrDefault(i => i.Id == index);
            }
        }
    }
}
