using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public abstract class RepositoryForRelations<T> where T: class
    {
        protected DbSet<T> DbSet { get; set; }
        protected IStorageContext StorageContext;
        protected IStorage Storage { get; set; }
        protected ILoggerFactory LoggerFactory;
        protected ILogger Logger;

        public void SetStorageContext(IStorageContext storageContext, IStorage storage, ILoggerFactory loggerFactory)
        {
            try
            {
                StorageContext = storageContext;
                Storage = storage;
                LoggerFactory = loggerFactory;
                Logger = LoggerFactory.CreateLogger(this.GetType().FullName);
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

    }
}
