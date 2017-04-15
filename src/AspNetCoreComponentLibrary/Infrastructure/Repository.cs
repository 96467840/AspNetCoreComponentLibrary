using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public abstract class Repository<K, T> where K : struct where T : BaseDM<K>
    {
        /*protected DbSet<T> _dbSet = null;
        protected DbSet<T> DbSet
        {
            get
            {
                //(Storage as Storage)._logger.LogInformation("try get dbSet...");
                if (_dbSet != null) return _dbSet;
                _dbSet = (StorageContext as DbContext).Set<T>();
                return _dbSet;
            }
        }/**/

        protected DbSet<T> DbSet { get; set; }

        protected IStorageContext StorageContext;
        protected IStorage Storage { get; set; }
        protected ILoggerFactory LoggerFactory;
        protected ILogger Logger;

        public void SetStorageContext(IStorageContext storageContext, IStorage storage, ILoggerFactory loggerFactory)
        {
            StorageContext = storageContext;
            Storage = storage;
            LoggerFactory = loggerFactory;
            Logger = LoggerFactory.CreateLogger(this.GetType().FullName);
            DbSet = (StorageContext as DbContext).Set<T>();
        }

        public virtual IQueryable<T> StartQuery() {
            return DbSet.AsNoTracking();
        }

        public virtual void Save(T item)
        {
            if (item == null) throw new ArgumentNullException();
            if (item.Id.HasValue)
            {
                DbSet.Update(item);
            }
            else
            {
                DbSet.Add(item);
            }
            Storage.Save();
            AfterSave(item);
        }

        public virtual void AfterSave(T item) {
            Logger.LogInformation("Repository AfterSave for {0}", item.Id);
        }

        public virtual void Remove(K id)
        {
            var item = this[id];
            if (item == null) return;
            DbSet.Remove(item);
            Storage.Save();
        }

        protected virtual void SetBlock(K id, bool value)
        {
            if (typeof(T) is IBlockable)
            {
                T item = (T)Activator.CreateInstance(typeof(T));
                item.Id = id;
                ((IBlockable)item).IsBlocked = value;
                DbSet.Update(item);
                Storage.Save();
            }
        }

        public void Block(K id) { SetBlock(id, true); }
        public void UnBlock(K id) { SetBlock(id, false); }

        public virtual T this[K? index]
        {
            get
            {
                if (index == null) return default(T);
                return DbSet.FirstOrDefault(i => i.Id.ToString() == index.ToString());
                //return dbSet.FirstOrDefault(i => i.Id == index);
            }
        }
    }
}
