using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public abstract class Repository<K, T> : IRepository<K, T> /*where K : struct/*, IComparable<K>*/ where T : BaseDM<K>
    {
        protected DbSet<T> DbSet { get; set; }

        protected IStorageContext StorageContext;
        protected IStorage Storage { get; set; }
        protected ILoggerFactory LoggerFactory;
        protected ILogger Logger;
        protected ILogger LoggerMEF;

        public void SetStorageContext(IStorageContext storageContext, IStorage storage, ILoggerFactory loggerFactory)
        {
            try
            {
                StorageContext = storageContext;
                Storage = storage;
                LoggerFactory = loggerFactory;
                Logger = LoggerFactory.CreateLogger(this.GetType().FullName);
                // для красоты в логах EntityFrameworkCore
                LoggerMEF = LoggerFactory.CreateLogger(Utils.MEFNameSpace);
                //if (StorageContext == null) Logger.LogCritical("AAAAAAAAAAAAAAAAAAA!!!!!!!!!");
                DbSet = (StorageContext as DbContext).Set<T>();
            }
            catch (Exception e)
            {
                Logger.LogCritical(e.ToString());
            }
        }

        public virtual IQueryable<T> StartQuery()
        {
            return DbSet.AsNoTracking();
        }

        public List<T> GetUnblocked(long siteid)
        {
            Logger.LogTrace("Repository GetUnblocked for {0}. IWithSiteId = {1}", GetType().FullName, typeof(IWithSiteId).GetTypeInfo().IsAssignableFrom(typeof(T)));
            if (typeof(T).IsImplementsInterface(typeof(IWithSiteId)))
                using (new BLog(LoggerMEF, "GetUnblocked", GetType().FullName))
                    return StartQuery().Where(i => ((IWithSiteId)i).SiteId == siteid).ToList();

            return new List<T>();
        }

        public virtual void Save(T item)
        {
            if (item == null) throw new ArgumentNullException();

            if (!BeforeSave(item)) return;

            var isnew = Utils.CheckDefault(item.Id);

            if (!isnew)
            {
                DbSet.Update(item);
            }
            else
            {
                DbSet.Add(item);
            }
        }

        public virtual bool BeforeSave(T item)
        {
            Logger.LogTrace("Repository BeforeSave for {0}", item.Id);
            return true;
        }

        public virtual void AfterSave(T item, bool isnew)
        {
            Logger.LogTrace("Repository AfterSave for {0}", item.Id);
        }/**/

        public virtual void Remove(T item)
        {
            DbSet.Remove(item);
        }

        protected virtual void SetBlock(K id, bool value)
        {
            //if (typeof(T) is IBlockable)
            if (typeof(T).IsImplementsInterface(typeof(IBlockable)))
            {
                T item = (T)Activator.CreateInstance(typeof(T));
                item.Id = id;
                ((IBlockable)item).IsBlocked = value;
                DbSet.Update(item);
            }
        }

        public void Block(K id) { SetBlock(id, true); }
        public void UnBlock(K id) { SetBlock(id, false); }

        //public virtual void AddToCache(K id) { }
        //public virtual void RemoveFromCache(K id) { }

        public virtual T this[K index]
        {
            get
            {
                return GetSingleFromDB(index);
            }
        }

        public virtual T GetSingleFromDB(K index)
        {
            using (new BLog(LoggerMEF, "GetSingleFromDB", GetType().FullName))
            {
                return DbSet
                    // очень важно!
                    .AsNoTracking()
                    .FirstOrDefault(i => i.Id.Equals(index));
            }
        }

        public virtual void ClearCache()
        {
            Logger.LogTrace("Repository ClearCache {0}", this.GetType().FullName);
        }

    }
}
