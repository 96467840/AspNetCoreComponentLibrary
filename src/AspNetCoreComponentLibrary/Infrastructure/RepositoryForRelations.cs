﻿using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public abstract class RepositoryForRelations<T> : IRepository<T> where T: class
    {
        protected DbSet<T> DbSet { get; set; }
        protected IStorageContext StorageContext;
        protected IStorage Storage { get; set; }
        protected ILoggerFactory LoggerFactory;
        protected ILogger Logger;
        protected ILogger LoggerMEF;
        protected ILocalizer2Garin Localizer2Garin;

        public void SetStorageContext(IStorageContext storageContext, IStorage storage, ILoggerFactory loggerFactory, ILocalizer2Garin localizer2Garin)
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
                Localizer2Garin = localizer2Garin;
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

        public virtual void Save(T item)
        {
            if (item == null) throw new ArgumentNullException();
            
            DbSet.Add(item);
            (StorageContext as DbContext).SaveChanges();
        }/**/

        public virtual void Remove(T item)
        {
            //var item = this[id];
            //if (item == null) return;
            DbSet.Remove(item);
            (StorageContext as DbContext).SaveChanges();
        }

        public void Save(List<T> items)
        {
            if (items == null || !items.Any()) return;
            DbSet.AddRange(items);
            (StorageContext as DbContext).SaveChanges();
        }

        public void Remove(List<T> items)
        {
            if (items == null || !items.Any()) return;
            DbSet.RemoveRange(items);
            (StorageContext as DbContext).SaveChanges();
        }
    }
}
